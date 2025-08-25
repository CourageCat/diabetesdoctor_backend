using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.ParticipantDtos;
using ChatService.Contract.Services.Conversation.Filters;

namespace ChatService.Contract.Services.Participant.Queries;

public record GetParticipantsByConversationQuery : IQuery<Response<PagedResult<ParticipantResponseDto>>>
{
    public string UserId { get; init; } = null!;
    public string Role { get; init; } = null!;
    public ObjectId ConversationId { get; init; }
    public OffsetPaginationRequest Pagination { get; init; } = null!;
    public GetParticipantsByConversationIdFilters Filters { get; init; } = null!; 
}