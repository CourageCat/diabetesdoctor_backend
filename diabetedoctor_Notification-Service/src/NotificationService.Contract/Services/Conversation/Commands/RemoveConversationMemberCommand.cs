namespace NotificationService.Contract.Services.Conversation.Commands;

public record RemoveConversationMemberCommand : ICommand
{
    public string ConversationId { get; init; } = null!;
    public string MemberId { get; init; } = null!;
};