using MediaService.Contract.EventBus.Abstractions.Message;

namespace MediaService.Contract.EventBus.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string? topic, TEvent @event, int retry, CancellationToken cancellation = default) where TEvent: IntegrationEvent;
}