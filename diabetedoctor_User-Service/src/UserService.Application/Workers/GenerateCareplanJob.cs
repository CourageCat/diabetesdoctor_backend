using Microsoft.Extensions.Logging;

namespace UserService.Application.Workers;

public class GenerateCareplanJob : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GenerateCareplanJob> _logger;

    public GenerateCareplanJob(IServiceProvider serviceProvider, ILogger<GenerateCareplanJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("GenerateCareplanJob chạy lúc: {Time}", DateTime.Now);

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Lấy tất cả template
        var templates = await db.CarePlanMeasurements
            .ToListAsync();

        // Sinh instance hợp lệ
        var instances = GenerateMeasurementInstancesForPatient(templates);

        if (instances.Any())
        {
            db.AddRange(instances);
            await db.SaveChangesAsync();
            _logger.LogInformation("Tạo {Count} CarePlanMeasurementInstance", instances.Count);
        }
        else
        {
            _logger.LogInformation("Không có instance nào cần tạo hôm nay");
        }
    }

    private static List<CarePlanMeasurementInstance> GenerateMeasurementInstancesForPatient(
        List<CarePlanMeasurementTemplate> templates)
    {
        var scheduledDate = DateTimeHelper.ToLocalTimeNow(NationEnum.VietNam).Date;
        var now = DateTime.UtcNow;

        var instances = new List<CarePlanMeasurementInstance>();

        foreach (var measurement in templates)
        {
            var time = measurement.ScheduledAt;
            var scheduledDateTime = DateTime.SpecifyKind(
                scheduledDate.Add(time.ToTimeSpan()),
                DateTimeKind.Utc
            );
            var scheduledDateTimeUtc = DateTimeHelper.ToUtcTime(NationEnum.VietNam, scheduledDateTime);

            if (scheduledDateTimeUtc <= now || scheduledDateTimeUtc - now <= TimeSpan.FromMinutes(30))
                continue;

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
}