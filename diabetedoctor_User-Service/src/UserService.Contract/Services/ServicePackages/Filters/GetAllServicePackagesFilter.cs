namespace UserService.Contract.Services.ServicePackages.Filters;

public class GetAllServicePackagesFilter
{
    public string? Search { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}