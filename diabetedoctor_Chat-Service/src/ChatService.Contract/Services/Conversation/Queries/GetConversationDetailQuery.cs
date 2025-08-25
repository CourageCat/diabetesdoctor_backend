using ChatService.Contract.DTOs.ConversationDtos.Responses;

namespace ChatService.Contract.Services.Conversation.Queries;

public record GetConversationDetailQuery : IQuery<Response<ConversationResponseDto>>
{
    public string UserId { get; init; } = null!;
    public string Role { get; init; } = null!;
    public ObjectId ConversationId { get; init; }
};