namespace UserService.Contract.Services.Doctors.Filters;

public record GetAllCarePlanTemplatesFilter
{
    public string? Search { get; init; }
    public RecordEnum? RecordType { get; init; }
    public HealthCarePlanSubTypeEnum? SubType { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}