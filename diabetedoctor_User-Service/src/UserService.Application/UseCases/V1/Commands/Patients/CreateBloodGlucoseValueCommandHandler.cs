using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class CreateBloodGlucoseValueCommandHandler
    : ICommandHandler<CreateBloodGlucoseValueCommand, Success<string>>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientProfileRepository;
    private readonly IRepositoryBase<HealthRecord, Guid> _healthRecordRepository;
    private readonly IPublisher _publisher;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBloodGlucoseValueCommandHandler(IRepositoryBase<PatientProfile, Guid> patientProfileRepository,
        IRepositoryBase<HealthRecord, Guid> healthRecordRepository, IPublisher publisher, IUnitOfWork unitOfWork)
    {
        _patientProfileRepository = patientProfileRepository;
        _healthRecordRepository = healthRecordRepository;
        _publisher = publisher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Success<string>>> Handle(CreateBloodGlucoseValueCommand command,
        CancellationToken cancellationToken)
    {
        var patientProfile = await _patientProfileRepository.FindSingleAsync
        (p => p.UserId == command.UserId,
            includeProperties: p => p.HealthRecords,
            cancellationToken: cancellationToken);

        if (patientProfile == null)
            return FailureFromMessage(PatientErrors.ProfileNotExist);

        var healthRecord = CreateHealthRecord(patientProfile.Id, command);
        Console.WriteLine(healthRecord.PatientProfileId);
        _healthRecordRepository.Add(healthRecord);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (command.CarePlanInstanceId.HasValue)
        {
            await _publisher.Publish(
                new HealthRecordCreatedDomainEvent(Guid.NewGuid(), healthRecord, command.CarePlanInstanceId.Value),
                cancellationToken);
        }

        return Result.Success(new Success<string>(HealthRecordMessages.CreateHealthRecordSuccessfully.GetMessage().Code,
            HealthRecordMessages.CreateHealthRecordSuccessfully.GetMessage().Message, healthRecord.Id.ToString()));
    }

    /// <summary>
    /// Tạo HealthRecord
    /// </summary>
    private static HealthRecord CreateHealthRecord(Guid patientId, CreateBloodGlucoseValueCommand command)
    {
        var measureTime = command.MeasureTime.ToEnum<BloodGlucoseMeasureTimeEnum, BloodGlucoseMeasureTimeType>();
        var value = BloodGlucoseValue.Of(command.Value, measureTime);
        var measurementAt = command.MeasurementAt != null
            ? DateTime.SpecifyKind((DateTime)command.MeasurementAt, DateTimeKind.Utc)
            : DateTime.UtcNow;
        var recordId = new UuidV7().Value;
        var record = HealthRecord.Create(patientId, RecordType.BloodGlucose, value, measuredAt: measurementAt,
            personNote: command.PersonNote, id: recordId);

        return record;
    }

    /// <summary>
    /// Creates a failure Result from an PatientMessages enum.
    /// </summary>
    private static Result<Success<string>> FailureFromMessage(Error error)
    {
        return Result.Failure<Success<string>>(error);
    }
}