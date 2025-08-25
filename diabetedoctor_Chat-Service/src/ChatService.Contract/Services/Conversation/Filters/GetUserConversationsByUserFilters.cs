using ChatService.Contract.Enums;

namespace ChatService.Contract.Services.Conversation.Filters;

public record GetUserConversationsByUserFilters
{
    public ConversationTypeEnum Type { get; init; }
}