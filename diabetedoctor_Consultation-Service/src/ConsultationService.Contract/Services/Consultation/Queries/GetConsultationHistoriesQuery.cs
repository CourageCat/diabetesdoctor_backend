using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Common.Pagination;
using ConsultationService.Contract.DTOs.ConsultationDtos.Responses;
using ConsultationService.Contract.Services.Consultation.Filters;

namespace ConsultationService.Contract.Services.Consultation.Queries;

public record GetConsultationHistoriesQuery : IQuery<Response<CursorPagedResult<ConsultationResponseDto>>>
{
    public string UserId { get; init; } = null!;
    public string Role { get; init; } = null!;
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetConsultationHistoriesFilter Filter { get; init; } = null!;
};