namespace UserService.Contract.Services.ServicePackages.Filters;

public class GetAllServicePackagesByAdminFilter
{
    public bool? IsActive { get; init; }
    public string? Search { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}