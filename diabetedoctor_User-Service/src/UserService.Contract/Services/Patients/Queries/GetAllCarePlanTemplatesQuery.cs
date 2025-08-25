using UserService.Contract.Common.Pagination;
using UserService.Contract.Services.Patients.Filters;
using UserService.Contract.Services.Patients.Responses;

namespace UserService.Contract.Services.Patients.Queries;

public record GetAllCarePlanTemplatesQuery : IQuery<Success<CursorPagedResult<CarePlanTemplateResponse>>>
{
    public Guid PatientId { get; init; }
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetAllCarePlanTemplatesFilter Filters { get; init; } = null!;
}