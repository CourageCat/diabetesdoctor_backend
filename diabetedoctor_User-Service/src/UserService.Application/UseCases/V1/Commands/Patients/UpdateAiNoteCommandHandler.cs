using UserService.Application.Helper;
using UserService.Contract.Common.DomainErrors;
using UserService.Contract.DTOs.HealthRecord;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class UpdateAiNoteCommandHandler : ICommandHandler<UpdateAiNoteCommand, Success>
{
    private readonly IRepositoryBase<HealthRecord, Guid> _healthRecordRepository;
    private readonly IRepositoryBase<PatientProfile, Guid> _patientProfileRepository;
    private readonly IAiService _aiService;

    public UpdateAiNoteCommandHandler(IRepositoryBase<HealthRecord, Guid> healthRecordRepository,
        IRepositoryBase<PatientProfile, Guid> patientProfileRepository, IAiService aiService)
    {
        _healthRecordRepository = healthRecordRepository;
        _patientProfileRepository = patientProfileRepository;
        _aiService = aiService;
    }

    public async Task<Result<Success>> Handle(UpdateAiNoteCommand command, CancellationToken cancellationToken)
    {
        var patientFound =
            await _patientProfileRepository.FindSingleAsync(p => p.UserId == command.UserId, true, cancellationToken);
        if (patientFound is null)
        {
            return FailureFromMessage(PatientErrors.ProfileNotExist);
        }

        var healthRecordFound =
            await _healthRecordRepository.FindSingleAsync(hr => hr.Id == command.HealthRecordId, true, cancellationToken);
        if (healthRecordFound is null)
        {
            return FailureFromMessage(HealthRecordErrors.HealthRecordNotFound);
        }

        if (healthRecordFound.PatientProfileId != patientFound.Id)
        {
            return FailureFromMessage(HealthRecordErrors.HealthRecordNotBelongToUser);
        }

        var request = HandleGenerateAiNote(healthRecordFound);

        var prompt = AiPromptExtension.BuildPromptAiNote(request);

        var aiNote = await _aiService.GenerateContentAsync(prompt, cancellationToken);

        if (aiNote is null)
        {
            return FailureFromMessage(HealthRecordErrors.GenerateAiNoteFailed);
        }

        healthRecordFound.UpdateAiNote(aiNote);

        return Result.Success(new Success(HealthRecordMessages.UpdateAiNoteSuccessfully.GetMessage(
        ).Code, HealthRecordMessages.UpdateAiNoteSuccessfully.GetMessage().Message));
    }

    private RequestForAiNoteDto HandleGenerateAiNote(HealthRecord healthRecord)
    {
        var recordValue = healthRecord.RecordValue;
        string measurementType = "";
        string value = "";
        string context = "";
        string time = DateTimeHelper.ToLocalTime(NationEnum.VietNam, healthRecord.MeasuredAt).ToString("HH:mm");
        string note = healthRecord.PersonNote ?? "";

        if (recordValue is BloodGlucoseValue bg)
        {
            measurementType = "Blood Glucose";
            value = $"{bg.Value} {bg.Unit}";
            context = bg.MeasureTimeType.ToString(); // ví dụ: Fasting, BeforeMeal,...
        }
        else if (recordValue is BloodPressureValue bp)
        {
            measurementType = "Blood Pressure";
            value = $"{bp.Systolic}/{bp.Diastolic} {bp.Unit}";
        }
        else if (recordValue is HeightValue h)
        {
            measurementType = "Height";
            value = $"{h.Value} {h.Unit}";
        }
        else if (recordValue is WeightValue w)
        {
            measurementType = "Weight";
            value = $"{w.Value} {w.Unit}";
        }
        else if (recordValue is HbA1cValue hba1c)
        {
            measurementType = "HbA1c";
            value = $"{hba1c.Value} {hba1c.Unit}";
        }

        var request = new RequestForAiNoteDto
        {
            MeasurementType = measurementType,
            Value = value,
            Time = time,
            Context = context,
            Note = note
        };

        return request;
    }

    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}