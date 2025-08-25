namespace UserService.Contract.Services.Patients.Filters;

public record GetAllCarePlanTemplatesFilter
{
    public string? Search { get; init; }
    public RecordEnum? RecordType { get; init; }
    public HealthCarePlanPeriodEnum? Period { get; init; }
    public HealthCarePlanSubTypeEnum? SubType { get; init; }
    public Guid? DoctorId { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}