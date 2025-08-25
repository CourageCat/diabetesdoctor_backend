using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;

public record ConsultationBookedIntegrationEvent : IntegrationEvent
{
    public string ConsultationId { get; init; } = null!;
    public string PatientId { get; init; } = null!;
    public string DoctorId { get; init; } = null!;
    public bool IsOpened { get; init; }
};