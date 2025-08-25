namespace NotificationService.Contract.EventBus.Events.Conversations;

public record ConversationUpdatedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public string ConversationName { get; init; }= null!;
    public string Avatar { get; init; } = null!;
}