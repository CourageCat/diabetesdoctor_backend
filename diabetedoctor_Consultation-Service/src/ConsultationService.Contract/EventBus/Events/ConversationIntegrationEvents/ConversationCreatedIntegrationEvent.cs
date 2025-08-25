using ChatService.Contract.Enums;
using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Events.ConversationIntegrationEvents;

public record ConversationCreatedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public ConversationTypeEnum ConversationType { get; init; }
    public bool IsOpened { get; init; }
    public string? ConsultationId { get; init; }
}