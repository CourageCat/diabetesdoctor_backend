using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Events.ConversationIntegrationEvents;

public record ConversationLinkedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public string ConsultationId { get; init; } = null!;
    public bool IsOpened { get; init; }
}