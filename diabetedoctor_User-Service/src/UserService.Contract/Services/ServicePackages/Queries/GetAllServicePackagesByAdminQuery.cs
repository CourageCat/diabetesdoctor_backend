using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.ServicePackages.Filters;
using UserService.Contract.Services.ServicePackages.Responses;

namespace UserService.Contract.Services.ServicePackages.Queries;

public class GetAllServicePackagesByAdminQuery : IQuery<Success<OffsetPagedResult<ServicePackageResponse>>>
{
    public OffsetPaginationRequest Pagination { get; init; } = null!;
    public GetAllServicePackagesByAdminFilter Filters { get; init; } = null!;
    public Guid AdminId { get; init; }
}