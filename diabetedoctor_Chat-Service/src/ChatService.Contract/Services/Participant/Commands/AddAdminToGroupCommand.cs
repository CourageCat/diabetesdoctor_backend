namespace ChatService.Contract.Services.Participant.Commands;

public record AddAdminToGroupCommand : ICommand<Response>
{
    public string StaffId { get; init; } = null!;
    public ObjectId ConversationId { get; init; }
    public string AdminId { get; init; } = null!;
}