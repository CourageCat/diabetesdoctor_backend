using System.Text.Json;
using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;

namespace AuthService.Api.Helpers;

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