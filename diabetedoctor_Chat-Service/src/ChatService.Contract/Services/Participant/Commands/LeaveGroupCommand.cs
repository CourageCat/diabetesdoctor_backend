namespace ChatService.Contract.Services.Participant.Commands;

public record LeaveGroupCommand : ICommand<Response>
{
    public string UserId { get; init; } = null!;
    public ObjectId ConversationId { get; init; }
}