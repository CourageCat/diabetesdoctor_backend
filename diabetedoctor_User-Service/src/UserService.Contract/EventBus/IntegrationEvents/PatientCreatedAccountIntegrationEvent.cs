namespace UserService.Contract.EventBus.IntegrationEvents;

public record PatientCreatedAccountIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; set; } = default!;
    public string? FullName { get; set; }
}