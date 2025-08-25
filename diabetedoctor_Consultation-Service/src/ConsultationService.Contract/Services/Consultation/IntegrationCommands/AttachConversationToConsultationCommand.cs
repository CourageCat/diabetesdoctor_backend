using ConsultationService.Contract.Abstractions.Message;
using MongoDB.Bson;

namespace ConsultationService.Contract.Services.Consultation.IntegrationCommands;

public record AttachConversationToConsultationCommand() : ICommand
{
    public string ConversationId { get; init; } = null!;
    public ObjectId ConsultationId { get; init; }
    public bool IsOpened { get; init; }
};