using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class DeleteCarePlanInstanceCommandHandler : ICommandHandler<DeleteCarePlanInstanceCommand, Success>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientRepository;
    private readonly IRepositoryBase<CarePlanMeasurementInstance, Guid> _carePlanMeasurementInstanceRepository;

    public DeleteCarePlanInstanceCommandHandler(IRepositoryBase<PatientProfile, Guid> patientRepository, IRepositoryBase<CarePlanMeasurementInstance, Guid> carePlanMeasurementInstanceRepository)
    {
        _patientRepository = patientRepository;
        _carePlanMeasurementInstanceRepository = carePlanMeasurementInstanceRepository;
    }

    public async Task<Result<Success>> Handle(DeleteCarePlanInstanceCommand command, CancellationToken cancellationToken)
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
            return Result.Failure<Success>(CarePlanInstanceErrors.CanNotDeleteCarePlanInstanceBelongToDoctor);
        }

        if (instanceFound.PatientProfileId != patientFound.Id)
        {
            return Result.Failure<Success>(CarePlanInstanceErrors.CarePlanInstanceNotBelongToPatient);
        }
        
        // Remove old instance in Background Job
        
        
        _carePlanMeasurementInstanceRepository.Remove(instanceFound);
        return Result.Success(new Success(
            CarePlanInstanceMessages.DeleteCarePlanInstanceSuccessfully.GetMessage().Code, 
            CarePlanInstanceMessages.DeleteCarePlanInstanceSuccessfully.GetMessage().Message));
    }
}