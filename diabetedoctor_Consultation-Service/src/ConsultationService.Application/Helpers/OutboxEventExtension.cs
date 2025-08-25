using System.Text.Json;
using ConsultationService.Contract.EventBus.Abstractions.Message;
using ConsultationService.Domain.Models;
using MongoDB.Bson;

namespace ConsultationService.Application.Helpers;

public static class OutboxEventExtension
{
    public static OutboxEvent ToOutboxEvent<TEvent>(string topic, TEvent @event, int retryCount = 0) where TEvent : IntegrationEvent
    {
        var id = ObjectId.GenerateNewId();
        var type = @event.GetType();
        var json = JsonSerializer.Serialize(@event, type);
        var retryMinutes = retryCount switch
        {
            1 => 10,
            2 => 30,
            _ => 60
        };
        return OutboxEvent.Create(id,  topic, type.Name, json, retryCount, retryMinutes);
    }
}