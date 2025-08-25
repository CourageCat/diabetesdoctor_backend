namespace ChatService.Contract.Services.Participant.Commands;

public record AddDoctorToGroupCommand : ICommand<Response>
{
    public string StaffId { get; init; } = null!;
    public ObjectId ConversationId { get; init; }
    public string DoctorId { get; init; } = null!;
}