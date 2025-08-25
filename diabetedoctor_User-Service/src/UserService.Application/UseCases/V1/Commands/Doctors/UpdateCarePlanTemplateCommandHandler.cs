using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Doctors.Commands;

namespace UserService.Application.UseCases.V1.Commands.Doctors;

public sealed class UpdateCarePlanTemplateCommandHandler : ICommandHandler<UpdateCarePlanTemplateCommand, Success>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientRepository;
    private readonly IRepositoryBase<DoctorProfile, Guid> _doctorRepository;
    private readonly IRepositoryBase<CarePlanMeasurementTemplate, Guid> _carePlanMeasurementRepository;

    public UpdateCarePlanTemplateCommandHandler(IRepositoryBase<PatientProfile, Guid> patientRepository, IRepositoryBase<DoctorProfile, Guid> doctorRepository, IRepositoryBase<CarePlanMeasurementTemplate, Guid> carePlanMeasurementRepository)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _carePlanMeasurementRepository = carePlanMeasurementRepository;
    }

    public async Task<Result<Success>> Handle(UpdateCarePlanTemplateCommand command, CancellationToken cancellationToken)
    {
        var patientFound =
            await _patientRepository.FindSingleAsync(patient => patient.UserId == command.PatientId, true, cancellationToken);
        if (patientFound is null)
        {
            return FailureFromMessage(PatientErrors.ProfileNotExist);
        }
        
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
            return FailureFromMessage(CarePlanTemplateErrors.CanNotUpdateCarePlanTemplateBelongToPatient);
        }

        if (templateFound.DoctorProfileId != doctorFound.Id)
        {
            return FailureFromMessage(CarePlanTemplateErrors.CarePlanTemplateNotBelongToDoctor);
        }
        
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var duplicatedTemplate = await _carePlanMeasurementRepository.FindSingleAsync(
            template => template.PatientProfileId == patientFound.Id && template.RecordType == recordType &&
                        template.ScheduledAt == command.ScheduledAt && template.DoctorProfileId == doctorFound.Id, true, cancellationToken);
        // Check if the index update is duplicated but the duplicated index is different from the index in template found 
        if (duplicatedTemplate is not null)
        {
            if (recordType != templateFound.RecordType ||
                command.ScheduledAt != templateFound.ScheduledAt)
            {
                return FailureFromMessage(CarePlanTemplateErrors.DuplicatedCarePlanTemplate);
            }
        }
        
        UpdateCarePlanMeasurementTemplate(templateFound, command);
        _carePlanMeasurementRepository.Update(templateFound);
        return Result.Success(new Success(
            CarePlanTemplateMessages.UpdateCarePlanTemplateSuccessfully.GetMessage().Code, 
            CarePlanTemplateMessages.UpdateCarePlanTemplateSuccessfully.GetMessage().Message));
    }

    private void UpdateCarePlanMeasurementTemplate(CarePlanMeasurementTemplate template, UpdateCarePlanTemplateCommand command)
    {
        // Convert Enum in Contract to Enum in Model
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var subType = command.SubType.ToEnumNullable<HealthCarePlanSubTypeEnum, HealthCarePlanSubTypeType>();
        template.Update(recordType, null, command.ScheduledAt, subType);
    }
    
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}