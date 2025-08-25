namespace MediaService.Contract.EventBus.Abstractions.Message;

public record IntegrationEvent : INotification
{
    public Guid EventId { get; set; } = new UuidV7().Value;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
}