namespace AuthService.Api.Persistences.Abstractions.Repositories;

public interface IOutboxEventConsumerRepository
{
    Task<bool> HasProcessedEventAsync(string eventId, string name, CancellationToken cancellationToken = default);
    void CreateEvent(OutboxEventConsumer eventConsumer);
}