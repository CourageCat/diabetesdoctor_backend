using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;

public record ConsultationEndedIntegrationEvent : IntegrationEvent
{
    public string ConversationId { get; init; } = null!;
    public string ConsultationId { get; init; } = null!;
    public string UserId { get; init; } = null!;
    public double ConsultationFee { get; init; }
};