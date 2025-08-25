using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.Doctors.Responses;
using UserService.Contract.Services.Hospitals.Filteres;

namespace UserService.Contract.Services.Hospitals.Queries;

public record GetAllDoctorsByStaffQuery : IQuery<Success<OffsetPagedResult<DoctorResponse>>>
{
    public Guid HospitalStaffId { get; init; }
    public OffsetPaginationRequest Pagination { get; init; } = null!;
    public GetAllDoctorsByStaffFilter Filters { get; init; } = null!;
}