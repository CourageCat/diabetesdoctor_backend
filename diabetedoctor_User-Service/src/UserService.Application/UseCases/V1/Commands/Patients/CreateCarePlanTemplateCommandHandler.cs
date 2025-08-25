using UserService.Contract.Common.DomainErrors;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Application.UseCases.V1.Commands.Patients;

public sealed class CreateCarePlanTemplateCommandHandler : ICommandHandler<CreateCarePlanTemplateCommand, Success>
{
    private readonly IRepositoryBase<PatientProfile, Guid> _patientRepository;
    private readonly IRepositoryBase<CarePlanMeasurementTemplate, Guid> _carePlanMeasurementRepository;
    private readonly IRepositoryBase<CarePlanMeasurementInstance, Guid>  _carePlanMeasurementInstanceRepository;

    public CreateCarePlanTemplateCommandHandler(IRepositoryBase<PatientProfile, Guid> patientRepository, IRepositoryBase<CarePlanMeasurementTemplate, Guid> carePlanMeasurementRepository, IRepositoryBase<CarePlanMeasurementInstance, Guid> carePlanMeasurementInstanceRepository)
    {
        _patientRepository = patientRepository;
        _carePlanMeasurementRepository = carePlanMeasurementRepository;
        _carePlanMeasurementInstanceRepository = carePlanMeasurementInstanceRepository;
    }


    public async Task<Result<Success>> Handle(CreateCarePlanTemplateCommand command, CancellationToken cancellationToken)
    {
        var patientFound =
            await _patientRepository.FindSingleAsync(patient => patient.UserId == command.PatientId, true, cancellationToken);
        if (patientFound is null)
        {
            return FailureFromMessage(PatientErrors.ProfileNotExist);
        }
        
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var duplicatedTemplate = await _carePlanMeasurementRepository.AnyAsync(
            template => template.PatientProfileId == patientFound.Id && template.RecordType == recordType &&
                        template.ScheduledAt == command.ScheduledAt && template.DoctorProfileId == null, cancellationToken);
        if (duplicatedTemplate)
        {
            return FailureFromMessage(CarePlanTemplateErrors.DuplicatedCarePlanTemplate);
        }
        
        var carePlanTemplate = CreateCarePlanMeasurementTemplate(patientFound.Id, command);
        _carePlanMeasurementRepository.Add(carePlanTemplate);
        return Result.Success(new Success(CarePlanTemplateMessages.CreateCarePlanTemplateSuccessfully.GetMessage().Code, CarePlanTemplateMessages.CreateCarePlanTemplateSuccessfully.GetMessage().Message));
    }

    private CarePlanMeasurementTemplate CreateCarePlanMeasurementTemplate(Guid patientId, CreateCarePlanTemplateCommand command)
    {
        // Convert Enum in Contract to Enum in Model
        var recordType = command.RecordType.ToEnum<RecordEnum, RecordType>();
        var subType = command.SubType.ToEnumNullable<HealthCarePlanSubTypeEnum, HealthCarePlanSubTypeType>();
        var carePlanTemplateId = Guid.NewGuid();
        var carePlanTemplate = CarePlanMeasurementTemplate.Create(carePlanTemplateId, patientId, recordType, null, command.ScheduledAt, subType, null, null);
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