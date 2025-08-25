namespace ChatService.Contract.Services.Conversation.IntegrationCommands;

public record ToggleGroupVisibilityCommand() : ICommand
{
    public ObjectId ConversationId { get; init; }
    public bool IsClosed {get; init;}
};