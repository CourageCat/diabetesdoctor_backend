namespace ChatService.Contract.Services.Conversation.Commands.GroupConversation;

public record RemoveGroupParticipantCommand : ICommand<Response>
{
    public string StaffId { get; init; } = null!;
    public ObjectId ConversationId { get; init; }
    public string MemberId { get; init; } = null!;
}