namespace MediaService.Contract.EventBus.Events.PostIntegrationEvents;

public record PostCreatedIntegrationEvent : IntegrationEvent
{
    public string PostId { get; set; }
    public string Title { get; set; }
    public string Thumbnail { get; set; }
}
