using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.ConversationDtos.Responses;
using ChatService.Contract.DTOs.ParticipantDtos;
using ChatService.Contract.Services.Conversation.Filters;

namespace ChatService.Contract.Services.Conversation.Queries;

public record GetAllConversationsInHospitalQuery : IQuery<Response<PagedResult<ConversationResponseDto>>>
{
    public string UserId { get; init; } = null!;
    public OffsetPaginationRequest Pagination { get; init; } = null!;
    public GetAllConversationsInHospitalFilters Filters { get; init; } = null!;
}