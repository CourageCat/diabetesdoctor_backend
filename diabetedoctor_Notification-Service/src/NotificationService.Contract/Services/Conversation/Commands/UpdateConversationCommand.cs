namespace NotificationService.Contract.Services.Conversation.Commands;

public record UpdateConversationCommand : ICommand
{
    public string ConversationId { get; init; } = null!;
    public string? Name { get; init; }
    public string? Avatar { get; init; }
}