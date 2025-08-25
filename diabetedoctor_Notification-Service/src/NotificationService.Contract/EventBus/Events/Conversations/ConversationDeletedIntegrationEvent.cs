namespace NotificationService.Contract.EventBus.Events.Conversations;

public record ConversationDeletedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
}