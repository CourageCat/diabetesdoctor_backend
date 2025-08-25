namespace UserService.Contract.Services.Hospitals.Filteres;

public record GetAllHospitalsByAdminFilter
{
    public string? Search { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}