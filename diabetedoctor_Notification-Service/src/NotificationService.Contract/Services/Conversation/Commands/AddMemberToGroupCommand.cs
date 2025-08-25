
namespace NotificationService.Contract.Services.Conversation.Commands;

public record AddMemberToGroupCommand : ICommand
{
    public string ConversationId { get; set; } = null!;
    public IEnumerable<string> Members { get; set; } = [];
}