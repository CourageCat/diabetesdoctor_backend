namespace ChatService.Contract.EventBus.Events.ConsultationIntegrationEvents;

public record ConsultationStartedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
};