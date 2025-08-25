namespace NotificationService.Contract.EventBus.Events.Notifications;

public record PostCreatedIntegrationEvent : IntegrationEvent
{
    public string PostId { get; init; }
    public string Title { get; init; }
    public string Thumbnail { get; init; }
}