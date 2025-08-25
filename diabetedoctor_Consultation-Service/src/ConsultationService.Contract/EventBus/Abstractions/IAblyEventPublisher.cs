using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Abstractions;

public interface IAblyEventPublisher
{
    Task PublishAsync<TEvent>(string? channelName, string? eventName, TEvent @event) where TEvent: IntegrationEvent;

}