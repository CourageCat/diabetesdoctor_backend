using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.ServicePackages.Filters;
using UserService.Contract.Services.ServicePackages.Responses;

namespace UserService.Contract.Services.ServicePackages.Queries;

public class GetAllServicePackagesQuery : IQuery<Success<CursorPagedResult<ServicePackageResponse>>>
{
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetAllServicePackagesFilter Filters { get; init; } = null!;
}