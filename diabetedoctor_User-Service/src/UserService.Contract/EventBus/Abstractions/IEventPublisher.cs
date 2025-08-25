namespace UserService.Contract.EventBus.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string? topic, TEvent @event, int retry, CancellationToken cancellation = default) where TEvent: IntegrationEvent;
}