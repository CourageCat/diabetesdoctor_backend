using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.ConversationDtos.Responses;
using ChatService.Contract.Services.Conversation.Filters;

namespace ChatService.Contract.Services.Conversation.Queries;

public record GetConversationsByUserQuery : IQuery<Response<CursorPagedResult<ConversationResponseDto>>>
{
    public string UserId { get; init; } = null!;
    public CursorPaginationRequest Pagination { get; init; } = null!;
    public GetUserConversationsByUserFilters Filters { get; init; } = null!;
}