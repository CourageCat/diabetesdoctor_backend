namespace ChatService.Contract.Services.Conversation.Commands.PersonalConversation;

public record CreateConsultationConversationCommand : ICommand
{
    public string ConsultationId { get; init; } = null!;
    public string PatientId { get; init; } = null!;
    public string DoctorId { get; init; } = null!;
    public bool IsOpened { get; init; }
}