using UserService.Contract.DTOs.Doctor;

namespace UserService.Contract.Services.Patients.Responses;

public record CarePlanTemplateResponse
{
    public string Id { get; init; } = null!;
    public RecordEnum RecordType { get; init; }
    public HealthCarePlanPeriodEnum? Period { get; init; }
    public TimeOnly? ScheduledAt { get; init; }
    public HealthCarePlanSubTypeEnum?  SubType { get; init; }
    public string? Reason  { get; init; }
    public DateTime? CreatedDate { get; init; }
    public DoctorDto? Doctor { get; init; }
}