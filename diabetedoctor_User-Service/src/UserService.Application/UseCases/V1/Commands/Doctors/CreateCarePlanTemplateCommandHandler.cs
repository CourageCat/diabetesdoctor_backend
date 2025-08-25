using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Doctors.Commands;

namespace UserService.Application.UseCases.V1.Commands.Doctors;

public sealed class CreateCarePlanTemplateCommandHandler : ICommandHandler<CreateCarePlanTemplateCommand, Success>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientRepository;
    private readonly IRepositoryBase<DoctorProfile, Guid> _doctorRepository;
    private readonly IRepositoryBase<CarePlanMeasurementTemplate, Guid> _carePlanMeasurementRepository;

    public CreateCarePlanTemplateCommandHandler(IRepositoryBase<PatientProfile, Guid> patientRepository, IRepositoryBase<DoctorProfile, Guid> doctorRepository, IRepositoryBase<CarePlanMeasurementTemplate, Guid> carePlanMeasurementRepository)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _carePlanMeasurementRepository = carePlanMeasurementRepository;
    }
    
    public async Task<Result<Success>> Handle(CreateCarePlanTemplateCommand command, CancellationToken cancellationToken)
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
        
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var duplicatedTemplate = await _carePlanMeasurementRepository.AnyAsync(
            template => template.PatientProfileId == patientFound.Id && template.RecordType == recordType && template.DoctorProfileId == doctorFound.Id &&
                        template.ScheduledAt == command.ScheduledAt, cancellationToken);
        if (duplicatedTemplate)
        {
            return FailureFromMessage(CarePlanTemplateErrors.DuplicatedCarePlanTemplate);
        }
        
        var carePlanTemplate = CreateCarePlanMeasurementTemplate(patientFound.Id, doctorFound.Id, command);
        _carePlanMeasurementRepository.Add(carePlanTemplate);
        return Result.Success(new Success(CarePlanTemplateMessages.CreateCarePlanTemplateSuccessfully.GetMessage().Code, CarePlanTemplateMessages.CreateCarePlanTemplateSuccessfully.GetMessage().Message));
    }

    private CarePlanMeasurementTemplate CreateCarePlanMeasurementTemplate(Guid patientId, Guid doctorId, CreateCarePlanTemplateCommand command)
    {
        // Convert Enum in Contract to Enum in Model
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var subType = command.SubType.ToEnumNullable<HealthCarePlanSubTypeEnum, HealthCarePlanSubTypeType>();
        var carePlanTemplateId = Guid.NewGuid();
        var reason = command.Reason ?? "Bác sĩ không đưa lí do";
        var carePlanTemplate = CarePlanMeasurementTemplate.Create(carePlanTemplateId, patientId, recordType, null, command.ScheduledAt, subType, reason, doctorId);
        return carePlanTemplate;
    }
    
    /// <summary>
    /// Creates a failure Result from an PatientMessages enum.
    /// </summary>
    private static Result<Success> FailureFromMessage(Error error)
    {
        return Result.Failure<Success>(error);
    }
}