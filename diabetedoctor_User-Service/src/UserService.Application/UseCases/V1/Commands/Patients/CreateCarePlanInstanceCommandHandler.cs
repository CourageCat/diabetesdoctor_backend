using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class CreateCarePlanInstanceCommandHandler : ICommandHandler<CreateCarePlanInstanceCommand, Success>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientRepository;
    private readonly IRepositoryBase<CarePlanMeasurementInstance, Guid> _carePlanInstanceRepository;

    public CreateCarePlanInstanceCommandHandler(IRepositoryBase<PatientProfile, Guid> patientRepository, IRepositoryBase<CarePlanMeasurementInstance, Guid> carePlanInstanceRepository)
    {
        _patientRepository = patientRepository;
        _carePlanInstanceRepository = carePlanInstanceRepository;
    }
    
    public async Task<Result<Success>> Handle(CreateCarePlanInstanceCommand command, CancellationToken cancellationToken)
    {
        var patientFound =
            await _patientRepository.FindSingleAsync(patient => patient.UserId == command.PatientId, true, cancellationToken);
        if (patientFound is null)
        {
            return Result.Failure<Success>(PatientErrors.ProfileNotExist);
        }
        
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var duplicatedInstance = await _carePlanInstanceRepository.AnyAsync(instance => instance.PatientProfileId == patientFound.Id && instance.RecordType == recordType && instance.ScheduledAt == command.ScheduledAt && instance.DoctorProfileId == null, cancellationToken);
        if (duplicatedInstance)
        {
            return Result.Failure<Success>(CarePlanInstanceErrors.DuplicatedCarePlanInstance);
        }

        var subtype = command.SubType.ToEnumNullable<HealthCarePlanSubTypeEnum, HealthCarePlanSubTypeType>();
        var instanceCreate =
            CreateCarePlanMeasurementInstance(recordType, subtype, command.ScheduledAt, patientFound.Id);
        _carePlanInstanceRepository.Add(instanceCreate);
        
        // Add new instance in Background Job for noti
        
        
        return Result.Success(new Success(CarePlanInstanceMessages.CreateCarePlanInstanceSuccessfully.GetMessage().Code, CarePlanInstanceMessages.CreateCarePlanInstanceSuccessfully.GetMessage().Message));
    }

    private CarePlanMeasurementInstance CreateCarePlanMeasurementInstance(RecordType recordType, HealthCarePlanSubTypeType? subtype, DateTime scheduledAt, Guid patientId)
    {
        var instanceId = new UuidV7().Value;
        return CarePlanMeasurementInstance.Create(instanceId, patientId, null, recordType, subtype, scheduledAt);
        
    }
}