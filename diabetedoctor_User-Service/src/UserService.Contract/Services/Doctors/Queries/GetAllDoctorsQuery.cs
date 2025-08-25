using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.Doctors.Filters;
using UserService.Contract.Services.Doctors.Responses;

namespace UserService.Contract.Services.Doctors.Queries;

public record GetAllDoctorsQuery : IQuery<Success<CursorPagedResult<DoctorResponse>>>
{
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetAllDoctorsFilter Filters { get; init; } = null!;
}