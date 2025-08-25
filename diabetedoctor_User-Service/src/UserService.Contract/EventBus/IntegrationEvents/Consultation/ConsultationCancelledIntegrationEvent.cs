namespace UserService.Contract.EventBus.IntegrationEvents.Consultation;

public record ConsultationCancelledIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public Guid UserPackageId {get; init;}
};