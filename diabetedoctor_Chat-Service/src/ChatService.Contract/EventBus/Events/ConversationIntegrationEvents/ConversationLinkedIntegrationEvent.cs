namespace ChatService.Contract.EventBus.Events.ConversationIntegrationEvents;

public record ConversationLinkedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public string ConsultationId { get; init; } = null!;
    public bool IsOpened { get; init; }
}