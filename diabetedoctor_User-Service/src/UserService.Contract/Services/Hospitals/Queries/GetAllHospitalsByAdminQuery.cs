using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.Hospitals.Filteres;
using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Contract.Services.Hospitals.Queries;

public record GetAllHospitalsByAdminQuery : IQuery<Success<OffsetPagedResult<HospitalResponse>>>
{
    public Guid AdminId { get; init; }
    public OffsetPaginationRequest Pagination { get; init; } = null!;
    public GetAllHospitalsByAdminFilter Filters { get; init; } = null!;
}