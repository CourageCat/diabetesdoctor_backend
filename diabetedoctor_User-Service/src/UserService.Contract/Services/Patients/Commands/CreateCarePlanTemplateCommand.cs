namespace UserService.Contract.Services.Patients.Commands;

public record CreateCarePlanTemplateCommand : ICommand<Success>
{
    public RecordEnum RecordType { get; init; }
    public TimeOnly ScheduledAt { get; init; }
    public HealthCarePlanSubTypeEnum? SubType { get; init; }
    public Guid PatientId { get; init; }
}