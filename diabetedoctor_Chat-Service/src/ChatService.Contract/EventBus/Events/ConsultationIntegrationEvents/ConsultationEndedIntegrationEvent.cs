namespace ChatService.Contract.EventBus.Events.ConsultationIntegrationEvents;

public record ConsultationEndedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
};