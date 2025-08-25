using System.Text.Json;
using UserService.Contract.EventBus.Abstractions.Messages;

namespace UserService.Application.Helper;

public static class OutboxEventExtension
{
    public static OutboxEvent ToOutboxEvent<TEvent>(string topic, TEvent @event, int retryCount = 0) where TEvent : IntegrationEvent
    {
        var id = Guid.NewGuid();
        var type = @event.GetType();
        var json = JsonSerializer.Serialize(@event);
        var retryMinutes = retryCount switch
        {
            1 => 10,
            2 => 30,
            _ => 60
        };
        return OutboxEvent.Create(id, topic, type.Name, json, retryCount, retryMinutes);
    }
}