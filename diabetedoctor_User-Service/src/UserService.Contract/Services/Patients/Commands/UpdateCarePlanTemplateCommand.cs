namespace UserService.Contract.Services.Patients.Commands;

public record UpdateCarePlanTemplateCommand : ICommand<Success>
{
    public Guid Id  { get; init; }
    public RecordEnum RecordType { get; init; }
    public TimeOnly ScheduledAt { get; init; }
    public HealthCarePlanSubTypeEnum? SubType { get; init; }
    public Guid PatientId { get; init; }

}