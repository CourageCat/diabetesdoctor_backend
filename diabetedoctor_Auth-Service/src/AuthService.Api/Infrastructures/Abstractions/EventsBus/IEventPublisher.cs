using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;

namespace AuthService.Api.Infrastructures.Abstractions.EventsBus;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(string? topic, TEvent @event, int retry, CancellationToken cancellation = default) where TEvent: IntegrationEvent;
}