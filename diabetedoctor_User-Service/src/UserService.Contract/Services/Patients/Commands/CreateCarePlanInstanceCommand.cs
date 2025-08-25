namespace UserService.Contract.Services.Patients.Commands;

public record CreateCarePlanInstanceCommand : ICommand<Success>
{
    public RecordEnum RecordType { get; init; }
    public HealthCarePlanSubTypeEnum? SubType { get; init; }
    public DateTime ScheduledAt { get; init; }
    public Guid PatientId { get; init; }
}