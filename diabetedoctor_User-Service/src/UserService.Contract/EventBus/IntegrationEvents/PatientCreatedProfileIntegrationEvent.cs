namespace UserService.Contract.EventBus.IntegrationEvents;

public record PatientCreatedProfileIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; set; } = default!;
}