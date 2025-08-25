using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;

public record ConsultationCancelledIntegrationEvent : IntegrationEvent
{
    public string UserId { get; init; } = null!;
    public string UserPackageId {get; init;} = null!;
    public string? Reason { get; init; }
};