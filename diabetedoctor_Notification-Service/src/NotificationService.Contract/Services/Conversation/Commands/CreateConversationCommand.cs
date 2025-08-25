namespace NotificationService.Contract.Services.Conversation.Commands;

public record CreateConversationCommand : ICommand
{
    public string ConversationId { get; init; } = null!;
    public string ConversationName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public IEnumerable<string> Members { get; init; } = [];
}