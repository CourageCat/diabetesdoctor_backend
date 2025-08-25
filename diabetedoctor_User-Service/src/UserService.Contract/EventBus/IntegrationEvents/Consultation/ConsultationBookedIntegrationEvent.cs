namespace UserService.Contract.EventBus.IntegrationEvents.Consultation;

public record ConsultationBookedIntegrationEvent : IntegrationEvent
{
    public Guid PatientId { get; init; }
};