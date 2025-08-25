namespace UserService.Contract.EventBus.IntegrationEvents.Hospital;

public record HospitalCreatedIntegrationEvent : IntegrationEvent
{
    public Guid HospitalId { get; init; }
    public string Name { get; init; } = null!;
    public string Thumbnail { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!;
    public string? Email { get; init; } = null!;
}