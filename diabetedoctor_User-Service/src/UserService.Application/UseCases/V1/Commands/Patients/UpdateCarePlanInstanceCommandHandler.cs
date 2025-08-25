using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class UpdateCarePlanInstanceCommandHandler : ICommandHandler<UpdateCarePlanInstanceCommand, Success>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientRepository;
    private readonly IRepositoryBase<CarePlanMeasurementInstance, Guid> _carePlanMeasurementInstanceRepository;

    public UpdateCarePlanInstanceCommandHandler(IRepositoryBase<PatientProfile, Guid> patientRepository, IRepositoryBase<CarePlanMeasurementInstance, Guid> carePlanMeasurementInstanceRepository)
    {
        _patientRepository = patientRepository;
        _carePlanMeasurementInstanceRepository = carePlanMeasurementInstanceRepository;
    }
    
    public async Task<Result<Success>> Handle(UpdateCarePlanInstanceCommand command, CancellationToken cancellationToken)
    {
        var patientFound =
            await _patientRepository.FindSingleAsync(patient => patient.UserId == command.PatientId, true, cancellationToken);
        if (patientFound is null)
        {
            return Result.Failure<Success>(PatientErrors.ProfileNotExist);
        }
        
        var instanceFound =
            await _carePlanMeasurementInstanceRepository.FindSingleAsync(instance => instance.Id == command.Id, true, cancellationToken);
        if (instanceFound is null)
        {
            return Result.Failure<Success>(CarePlanInstanceErrors.CarePlanInstanceNotExist);
        }

        if (instanceFound.DoctorProfileId != null)
        {
            return Result.Failure<Success>(CarePlanInstanceErrors.CanNotUpdateCarePlanInstanceBelongToDoctor);
        }

        if (instanceFound.PatientProfileId != patientFound.Id)
        {
            return Result.Failure<Success>(CarePlanInstanceErrors.CarePlanInstanceNotBelongToPatient);
        }
        
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var duplicatedInstance = await _carePlanMeasurementInstanceRepository.FindSingleAsync(
            instance => instance.PatientProfileId == patientFound.Id && instance.RecordType == recordType &&
                        instance.ScheduledAt == command.ScheduledAt && instance.DoctorProfileId == null, true, cancellationToken);
        var isNewInstanceLikeOldInstance = false;
        // Check if the index update is duplicated but the duplicated index is different from the index in instance found 
        if (duplicatedInstance is not null)
        {
            if (recordType != instanceFound.RecordType ||
                command.ScheduledAt != instanceFound.ScheduledAt)
            {
                return Result.Failure<Success>(CarePlanInstanceErrors.DuplicatedCarePlanInstance);
            }
            // New instance is the same with old instance
            isNewInstanceLikeOldInstance = true;
        }
        
        UpdateCarePlanMeasurementInstance(instanceFound, command);
        _carePlanMeasurementInstanceRepository.Update(instanceFound);
        
        if (!isNewInstanceLikeOldInstance)
        {
            // Remove old instance in Background Job
        
            // Add new instance in Background Job for noti
        }
        
        return Result.Success(new Success(
            CarePlanInstanceMessages.UpdateCarePlanInstanceSuccessfully.GetMessage().Code, 
            CarePlanInstanceMessages.UpdateCarePlanInstanceSuccessfully.GetMessage().Message));
    }

    private void UpdateCarePlanMeasurementInstance(CarePlanMeasurementInstance instance, UpdateCarePlanInstanceCommand command)
    {
        // Convert Enum in Contract to Enum in Model
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var subType = command.SubType.ToEnumNullable<HealthCarePlanSubTypeEnum, HealthCarePlanSubTypeType>();
        instance.Update(recordType, subType, command.ScheduledAt);
    }
}