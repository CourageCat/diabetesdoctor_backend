using UserService.Contract.DTOs.Doctor;

namespace UserService.Contract.Services.Doctors.Responses;

public record CarePlanInstanceResponse
{
    public string Id { get; init; } = null!;
    public RecordEnum RecordType { get; init; }
    public HealthCarePlanPeriodEnum? Period { get; init; }
    public HealthCarePlanSubTypeEnum? Subtype { get; init; }
    public string? Reason { get; init; }
    public DateTime? ScheduledAt { get; init; }
    public DateTime? MeasuredAt { get; init; }
    public bool? IsCompleted { get; init; }
    public DoctorDto? Doctor { get; init; }
}