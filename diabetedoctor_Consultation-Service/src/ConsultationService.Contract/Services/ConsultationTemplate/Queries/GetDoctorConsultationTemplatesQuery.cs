using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Common.Pagination;
using ConsultationService.Contract.Services.ConsultationTemplate.Filters;
using ConsultationService.Contract.Services.ConsultationTemplate.Responses;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Queries;

public record GetDoctorConsultationTemplatesQuery : IQuery<Response<CursorPagedResult<GetDoctorConsultationTemplatesResponse>>>
{
    public string UserId { get; init; } = null!;
    public string Role { get; init; } = null!;
    public string DoctorId { get; init; } = null!;
    public GetDoctorConsultationTemplatesFilter Filter { get; init; } = null!;
};