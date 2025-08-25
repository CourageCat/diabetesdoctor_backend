using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Doctors.Commands;

namespace UserService.Application.UseCases.V1.Commands.Doctors;

public sealed class DeleteCarePlanTemplateCommandHandler : ICommandHandler<DeleteCarePlanTemplateCommand, Success>
{
    private readonly IRepositoryBase<DoctorProfile, Guid> _doctorRepository;
    private readonly IRepositoryBase<CarePlanMeasurementTemplate, Guid> _carePlanMeasurementRepository;

    public DeleteCarePlanTemplateCommandHandler(IRepositoryBase<DoctorProfile, Guid> doctorRepository, IRepositoryBase<CarePlanMeasurementTemplate, Guid> carePlanMeasurementRepository)
    {
        _doctorRepository = doctorRepository;
        _carePlanMeasurementRepository = carePlanMeasurementRepository;
    }
    public async Task<Result<Success>> Handle(DeleteCarePlanTemplateCommand command, CancellationToken cancellationToken)
    {
        var doctorFound =
            await _doctorRepository.FindSingleAsync(patient => patient.UserId == command.DoctorId, true, cancellationToken);
        if (doctorFound is null)
        {
            return FailureFromMessage(DoctorErrors.ProfileNotExist);
        }
        var templateFound =
            await _carePlanMeasurementRepository.FindSingleAsync(template => template.Id == command.Id, true, cancellationToken);
        if (templateFound is null)
        {
            return FailureFromMessage(CarePlanTemplateErrors.CarePlanTemplateNotExist);
        }
        if (templateFound.DoctorProfileId == null)
        {
            return FailureFromMessage(CarePlanTemplateErrors.CanNotDeleteCarePlanTemplateBelongToPatient);
        }

        if (templateFound.DoctorProfileId != doctorFound.Id)
        {
            return FailureFromMessage(CarePlanTemplateErrors.CarePlanTemplateNotBelongToDoctor);
        }
        _carePlanMeasurementRepository.Remove(templateFound);
        return Result.Success(new Success(
            CarePlanTemplateMessages.DeleteCarePlanTemplateSuccessfully.GetMessage().Code, 
            CarePlanTemplateMessages.DeleteCarePlanTemplateSuccessfully.GetMessage().Message));
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}