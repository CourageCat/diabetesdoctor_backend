using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.ServicePackages.Filters;
using UserService.Contract.Services.ServicePackages.Responses;

namespace UserService.Contract.Services.ServicePackages.Queries;

public record GetAllServicePackagesPurchasedQuery : IQuery<Success<CursorPagedResult<ServicePackagePurchasedResponse>>>
{
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetAllServicePackagesPurchasedFilter Filters { get; init; } = null!;
    public Guid UserId { get; init; }
}