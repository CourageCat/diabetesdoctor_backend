using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;

public record ConsultationStartedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public string ConsultationId { get; init; } = null!;
}