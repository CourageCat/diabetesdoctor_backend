namespace ChatService.Contract.Services.Participant.Commands;

public record JoinGroupCommand : ICommand<Response>
{
    public ObjectId ConversationId { get; init; }
    public string InvitedBy { get; init; } = null!;
    public string UserId { get; init; } = null!;
}