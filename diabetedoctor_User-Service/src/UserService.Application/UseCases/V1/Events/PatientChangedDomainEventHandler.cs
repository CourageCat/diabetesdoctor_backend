using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Helper;
using UserService.Contract.EventBus.IntegrationEvents;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Events;

public sealed class PatientChangedDomainEventHandler
    : IDomainEventHandler<PatientProfileCreatedDomainEvent>
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PatientChangedDomainEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAiService _aiService;

    public PatientChangedDomainEventHandler(IBackgroundTaskQueue taskQueue, IServiceScopeFactory scopeFactory,
        ILogger<PatientChangedDomainEventHandler> logger, IUnitOfWork unitOfWork, IAiService aiService)
    {
        _taskQueue = taskQueue;
        _scopeFactory = scopeFactory;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _aiService = aiService;
    }

    public Task Handle(PatientProfileCreatedDomainEvent @event, CancellationToken cancellationToken)
    {
        _taskQueue.QueueBackgroundWorkItem(async token =>
        {
            using var scope = _scopeFactory.CreateScope();

            try
            {
                var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var carePlantemplateRepo = scope.ServiceProvider
                    .GetRequiredService<IRepositoryBase<CarePlanMeasurementTemplate, Guid>>();
                var carePlanInstanceRepo = scope.ServiceProvider
                    .GetRequiredService<IRepositoryBase<CarePlanMeasurementInstance, Guid>>();

                var patientRepo = scope.ServiceProvider.GetRequiredService<IRepositoryBase<PatientProfile, Guid>>();

                await GenerateCarePlanAsync(@event.PatientId, httpClientFactory, unitOfWork,
                    carePlantemplateRepo, carePlanInstanceRepo, patientRepo, @event.PatientProfile, token);
                await _unitOfWork.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to generate care plan in background for PatientProfileId: {PatientProfileId}",
                    @event.PatientProfile);
            }
        });

        return Task.CompletedTask;
    }

    private async Task GenerateCarePlanAsync(
        Guid patientId,
        IHttpClientFactory httpClientFactory,
        IUnitOfWork unitOfWork,
        IRepositoryBase<CarePlanMeasurementTemplate, Guid> carePlanRepo,
        IRepositoryBase<CarePlanMeasurementInstance, Guid> carePlanInstanceRepo,
        IRepositoryBase<PatientProfile, Guid> patientRepo,
        CreatePatientProfileCommand command,
        CancellationToken cancellationToken)
    {
        var age = DateTime.Today.Year - command.DateOfBirth.Year;
        if (command.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

        var gender = command.Gender.GetDescription();

        var h = command.HeightCm / 100f;
        float bmi = (float)(command.WeightKg / (h * h));

        var diabetesType = command.Diabetes.GetDescription();
        var insulinSchedule = command.InsulinInjectionFrequency?
            .GetDescription();

        var complications = command.Complications?.Count != 0
            ? (command.Complications?
                .Select(c => c.GetDescription())
                .ToList() ?? new List<string>())
            : new List<string>();

        if (!string.IsNullOrWhiteSpace(command.OtherComplicationDescription))
        {
            complications.Add(command.OtherComplicationDescription);
        }

        var pastDiseases = command.MedicalHistories?
            .Select(p => p.GetDescription())
            .ToList() ?? new List<string>();

        var lifestyle = command.ExerciseFrequency
            ?.GetDescription();

        var treatmentMethod = command.Diabetes == DiabetesEnum.Type1
            ? TreatmentMethodEnum.InsulinInjection.GetDescription()
            : command.Type2TreatmentMethod
                ?.GetDescription();

        var request = new CarePlanRequestDto(
            PatientId: patientId.ToString(),
            Age: age,
            Gender: gender,
            Bmi: bmi,
            DiabetesType: diabetesType,
            InsulinSchedule: insulinSchedule ?? "Không có tần suất tiêm Insulin",
            Complications: complications,
            TreatmentMethod: treatmentMethod ?? "Không có phương pháp điều trị",
            PastDiseases: pastDiseases,
            Lifestyle: lifestyle ?? "Không có tập thể dục"
        );
        var propmt = AiPromptExtension.BuildPromptCarePlan(request);
        var response = await _aiService.GenerateContentAsync(propmt, cancellationToken);
        try
        {
            var carePlanDtos = ConvertAiResponseToListCarePlans(response);
            if (carePlanDtos == null)
            {
                return;
            }
            var carePlans = carePlanDtos.Select(x =>
            {
                var templateId = new UuidV7().Value;
                return CarePlanMeasurementTemplate.Create(
                    templateId,
                    patientProfileId: patientId,
                    recordType: x.RecordType.ToEnum<RecordEnum, RecordType>(),
                    period: x.Period.ToEnum<HealthCarePlanPeriodEnum, HealthCarePlanPeriodType>(),
                    scheduledAt: MapPeriodToTime(x.Period.ToEnum<HealthCarePlanPeriodEnum, HealthCarePlanPeriodType>()),
                    subtype: x.Subtype is not null
                        ? x.Subtype.ToEnumNullable<HealthCarePlanSubTypeEnum, HealthCarePlanSubTypeType>()
                        : null,
                    reason: x.Reason.IsNullOrEmpty() == false ? x.Reason : null,
                    doctorProfileId: null);
            }).ToList();

            var carePlanInstances = GenerateMeasurementInstancesForPatient(carePlans);

            carePlanInstanceRepo.AddRange(carePlanInstances);
            carePlanRepo.AddRange(carePlans);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // ignored
        }
    }

    private static List<CarePlanMeasurementInstance> GenerateMeasurementInstancesForPatient(
        List<CarePlanMeasurementTemplate> templates)
    {
        // Chuyển Utc thành giờ Việt Nam để dùng hàm MapPeriodToTime, sau đó chuyển ngược về giờ Utc để lưu vào DB
        var scheduledDate = DateTimeHelper.ToLocalTimeNow(NationEnum.VietNam).Date;
        var now = DateTime.UtcNow;

        var instances = new List<CarePlanMeasurementInstance>();

        foreach (var measurement in templates)
        {
            var time = MapPeriodToTime(measurement.Period); // TimeOnly

            var scheduledDateTime = DateTime.SpecifyKind(
                scheduledDate.Add(time.ToTimeSpan()), // cộng giờ vào ngày
                DateTimeKind.Utc
            );
            
            var scheduledDateTimeUtc = DateTimeHelper.ToUtcTime(NationEnum.VietNam, scheduledDateTime);
            
            // Nếu thời gian đo nhỏ hơn thời gian hiện tại hoặc thời gian đo chỉ hơn thời gian hiện tại dưới 30 phút thì không thêm lịch đo
            if (scheduledDateTimeUtc <= now || (scheduledDateTimeUtc - now) <= TimeSpan.FromMinutes(30))
            {
                continue;
            }
            
            var instanceId = new UuidV7().Value;
            instances.Add(CarePlanMeasurementInstance.CreateFromTemplate(
                instanceId,
                template: measurement,
                patientProfileId: measurement.PatientProfileId,
                scheduledAt: scheduledDateTimeUtc
            ));
        }

        return instances;
    }

    private static TimeOnly MapPeriodToTime(HealthCarePlanPeriodType? period)
    {
        return period switch
        {
            // Giờ Việt Nam
            HealthCarePlanPeriodType.Morning => new TimeOnly(5, 30),
            HealthCarePlanPeriodType.Noon => new TimeOnly(11, 30),
            HealthCarePlanPeriodType.Evening => new TimeOnly(17, 0),
            HealthCarePlanPeriodType.BeforeBreakfast => new TimeOnly(7, 0),
            HealthCarePlanPeriodType.AfterBreakfast => new TimeOnly(8, 30),
            HealthCarePlanPeriodType.BeforeLunch => new TimeOnly(11, 30),
            HealthCarePlanPeriodType.AfterLunch => new TimeOnly(13, 30),
            HealthCarePlanPeriodType.Afternoon => new TimeOnly(15, 0),
            HealthCarePlanPeriodType.BeforeDinner => new TimeOnly(17, 30),
            HealthCarePlanPeriodType.AfterDinner => new TimeOnly(20, 0),
            HealthCarePlanPeriodType.BeforeSleep => new TimeOnly(22, 30),
            _ => new TimeOnly(8, 0), // default fallback
        };
    }

    private PatientCreatedProfileIntegrationEvent MapToPatientCreatedProfileIntegrationEvent(
        PatientProfile patientProfile)
    {
        return new PatientCreatedProfileIntegrationEvent
            { UserId = patientProfile.UserId };
    }

    private List<CarePlanMeasurementItemDto>? ConvertAiResponseToListCarePlans(string? aiResponse)
    {
        var cleanedJson = aiResponse!.Trim();
        if (cleanedJson.StartsWith("```"))
        {
            var start = cleanedJson.IndexOf('[');
            var end = cleanedJson.LastIndexOf(']');
            if (start >= 0 && end >= start)
                cleanedJson = cleanedJson.Substring(start, end - start + 1);
        }

        var options = new JsonSerializerOptions
        {
            Converters =
            {
                new JsonStringEnumConverter(),
                new NullableStringEnumConverter<HealthCarePlanSubTypeEnum>()
            }
        };
        return JsonSerializer.Deserialize<List<CarePlanMeasurementItemDto>>(cleanedJson, options);
    }
}