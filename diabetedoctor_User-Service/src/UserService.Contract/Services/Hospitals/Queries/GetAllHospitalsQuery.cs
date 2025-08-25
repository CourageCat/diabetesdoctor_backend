using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.Hospitals.Filteres;
using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Contract.Services.Hospitals.Queries;

public record GetAllHospitalsQuery : IQuery<Success<CursorPagedResult<HospitalResponse>>>
{
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetAllHospitalsFilter Filters { get; init; } = null!;
}