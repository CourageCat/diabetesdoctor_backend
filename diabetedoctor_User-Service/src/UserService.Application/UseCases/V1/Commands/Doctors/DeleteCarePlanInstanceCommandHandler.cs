using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Doctors.Commands;

namespace UserService.Application.UseCases.V1.Commands.Doctors;

public sealed class DeleteCarePlanInstanceCommandHandler : ICommandHandler<DeleteCarePlanInstanceCommand, Success>
{
    private readonly IRepositoryBase<DoctorProfile, Guid> _doctorRepository;
    private readonly IRepositoryBase<CarePlanMeasurementInstance, Guid> _carePlanMeasurementInstanceRepository;

    public DeleteCarePlanInstanceCommandHandler(IRepositoryBase<DoctorProfile, Guid> doctorRepository, IRepositoryBase<CarePlanMeasurementInstance, Guid> carePlanMeasurementInstanceRepository)
    {
        _doctorRepository = doctorRepository;
        _carePlanMeasurementInstanceRepository = carePlanMeasurementInstanceRepository;
    }

    public async Task<Result<Success>> Handle(DeleteCarePlanInstanceCommand command, CancellationToken cancellationToken)
    {
        var doctorFound =
            await _doctorRepository.FindSingleAsync(patient => patient.UserId == command.DoctorId, true, cancellationToken);
        if (doctorFound is null)
        {
            return Result.Failure<Success>(DoctorErrors.ProfileNotExist);
        }
        
        var instanceFound =
            await _carePlanMeasurementInstanceRepository.FindSingleAsync(instance => instance.Id == command.Id, true, cancellationToken);
        if (instanceFound is null)
        {
            return Result.Failure<Success>(CarePlanInstanceErrors.CarePlanInstanceNotExist);
        }
        
        if (instanceFound.DoctorProfileId == null)
        {
            return Result.Failure<Success>(CarePlanInstanceErrors.CanNotDeleteCarePlanInstanceBelongToPatient);
        }

        if (instanceFound.DoctorProfileId != doctorFound.Id)
        {
            return Result.Failure<Success>(CarePlanInstanceErrors.CarePlanInstanceNotBelongToDoctor);
        }
        
        // Remove old instance in Background Job
        
        
        _carePlanMeasurementInstanceRepository.Remove(instanceFound);
        return Result.Success(new Success(
            CarePlanInstanceMessages.DeleteCarePlanInstanceSuccessfully.GetMessage().Code, 
            CarePlanInstanceMessages.DeleteCarePlanInstanceSuccessfully.GetMessage().Message));
    }
}