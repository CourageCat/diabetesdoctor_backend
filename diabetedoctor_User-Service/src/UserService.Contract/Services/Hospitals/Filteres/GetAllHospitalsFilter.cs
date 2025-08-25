namespace UserService.Contract.Services.Hospitals.Filteres;

public record GetAllHospitalsFilter
{
    public string? Search { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}