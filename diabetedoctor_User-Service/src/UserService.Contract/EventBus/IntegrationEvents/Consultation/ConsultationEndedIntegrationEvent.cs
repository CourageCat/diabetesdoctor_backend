namespace UserService.Contract.EventBus.IntegrationEvents.Consultation;

public record ConsultationEndedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public double ConsultationFee { get; init; }
};