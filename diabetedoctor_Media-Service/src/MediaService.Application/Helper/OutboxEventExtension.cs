using System.Text.Json;
using MediaService.Contract.EventBus.Abstractions.Message;
using MediaService.Domain.Models;
using MongoDB.Bson;

namespace MediaService.Application.Helper;

public static class OutboxEventExtension
{
    public static OutboxEvent ToOutboxEvent<TEvent>(string topic, TEvent @event, int retryCount = 0) where TEvent : IntegrationEvent
    {
        var id = ObjectId.GenerateNewId();
        var type = @event.GetType();
        var json = JsonSerializer.Serialize(@event);
        var retryMinutes = retryCount switch
        {
            1 => 10,
            2 => 30,
            _ => 60
        };
        return OutboxEvent.Create(id,  topic, type.Name, json, retryCount, retryMinutes);
    }
}