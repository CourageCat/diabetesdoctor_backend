using MediaService.Domain.Models;
using MongoDB.Driver;

namespace MediaService.Domain.Abstractions.Repositories;
public interface IOutboxEventConsumerRepository
{
    Task<bool> HasProcessedEventAsync(string eventId, string name, CancellationToken cancellationToken = default);
    Task CreateEventAsync(IClientSessionHandle clientSession, OutboxEventConsumer eventConsumer, CancellationToken cancellationToken = default);
}