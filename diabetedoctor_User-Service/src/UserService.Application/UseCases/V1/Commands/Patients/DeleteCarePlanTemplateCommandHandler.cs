using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class DeleteCarePlanTemplateCommandHandler : ICommandHandler<DeleteCarePlanTemplateCommand, Success>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientRepository;
    private readonly IRepositoryBase<CarePlanMeasurementTemplate, Guid> _carePlanMeasurementRepository;

    public DeleteCarePlanTemplateCommandHandler(IRepositoryBase<PatientProfile, Guid> patientRepository, IRepositoryBase<CarePlanMeasurementTemplate, Guid> carePlanMeasurementRepository)
    {
        _patientRepository = patientRepository;
        _carePlanMeasurementRepository = carePlanMeasurementRepository;
    }
    public async Task<Result<Success>> Handle(DeleteCarePlanTemplateCommand command, CancellationToken cancellationToken)
    {
        var patientFound =
            await _patientRepository.FindSingleAsync(patient => patient.UserId == command.PatientId, true, cancellationToken);
        if (patientFound is null)
        {
            return FailureFromMessage(PatientErrors.ProfileNotExist);
        }
        
        var templateFound =
            await _carePlanMeasurementRepository.FindSingleAsync(template => template.Id == command.Id, true, cancellationToken);
        if (templateFound is null)
        {
            return FailureFromMessage(CarePlanTemplateErrors.CarePlanTemplateNotExist);
        }
        if (templateFound.DoctorProfileId != null)
        {
            return FailureFromMessage(CarePlanTemplateErrors.CanNotDeleteCarePlanTemplateBelongToDoctor);
        }

        if (templateFound.PatientProfileId != patientFound.Id)
        {
            return FailureFromMessage(CarePlanTemplateErrors.CarePlanTemplateNotBelongToPatient);
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