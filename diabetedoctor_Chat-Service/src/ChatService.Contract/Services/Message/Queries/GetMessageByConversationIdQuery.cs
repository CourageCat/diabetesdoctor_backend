using ChatService.Contract.Common.Pagination;
using ChatService.Contract.DTOs.MessageDtos;

namespace ChatService.Contract.Services.Message.Queries;

public record GetMessageByConversationIdQuery : IQuery<Response<CursorPagedResult<MessageResponseDto>>>
{
    public string UserId { get; init; } = null!;
    public ObjectId ConversationId { get; init; }
    
    public CursorPaginationRequest Pagination { get; init; } = null!;
}