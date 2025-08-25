using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.Hospitals.Filteres;
using UserService.Contract.Services.Hospitals.Responses;

namespace UserService.Contract.Services.Hospitals.Queries;

public record GetAllHospitalStaffsByAdminQuery : IQuery<Success<OffsetPagedResult<HospitalStaffResponse>>>
{
    public Guid HospitalAdminId { get; init; }
    public OffsetPaginationRequest Pagination { get; init; } = null!;
    public GetAllHospitalStaffsByAdminFilter Filters { get; init; } = null!;
}