using NotificationService.Contract.EventBus.Abstractions;

namespace NotificationService.Infrastructure.EventBus.Kafka;

public class KafkaEventPublisher(IProducer<string, EventEnvelope> producer, ILogger logger) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(string? topic, TEvent @event, int retry, CancellationToken cancellation = default) where TEvent : IntegrationEvent
    {
        var json = JsonSerializer.Serialize(@event, @event.GetType());
        logger.LogInformation("Publishing event {type} to topic {topic}: {event}", @event.GetType().Name, topic, json);

        try
        {
            await producer.ProduceAsync(topic, new Message<string, EventEnvelope>
            {
                Key = @event.EventId.ToString(),
                Value = new EventEnvelope(@event.GetType(), json, retry)
            }, cancellation);

            logger.LogInformation("Published event {@event}", @event.EventId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error publishing event {@event}", @event.EventId);
        }
    }
}