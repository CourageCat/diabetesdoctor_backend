using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.Doctors.Filters;
using UserService.Contract.Services.Doctors.Responses;

namespace UserService.Contract.Services.Doctors.Queries;

public record GetAllCarePlanTemplatesQuery : IQuery<Success<CursorPagedResult<CarePlanTemplateResponse>>>
{
    public Guid PatientId { get; init; }
    public Guid DoctorId { get; init; }
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetAllCarePlanTemplatesFilter Filters { get; init; } = null!;
}