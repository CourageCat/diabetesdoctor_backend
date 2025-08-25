namespace UserService.Contract.Services.ServicePackages.Filters;

public class GetAllServicePackagesPurchasedFilter
{
    public string? Search { get; init; }
    public string SortBy { get; init; } = string.Empty;
    public bool? IsExpired { get; init; } 
    public bool? IsExistedSessions { get; init; }
    public SortDirectionEnum SortDirection { get; init; } = SortDirectionEnum.Asc;
}