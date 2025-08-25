namespace UserService.Contract.Services.Doctors.Commands;

public record UpdateCarePlanInstanceCommand  : ICommand<Success>
{
    public Guid Id  { get; init; }
    public RecordEnum RecordType { get; init; }
    public HealthCarePlanSubTypeEnum? SubType { get; init; }
    public DateTime ScheduledAt { get; init; }
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
}