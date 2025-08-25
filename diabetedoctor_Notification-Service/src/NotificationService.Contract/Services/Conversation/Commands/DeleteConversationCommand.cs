namespace NotificationService.Contract.Services.Conversation.Commands;

public record DeleteConversationCommand : ICommand
{
    public string ConversationId { get; init; } = null!;
}