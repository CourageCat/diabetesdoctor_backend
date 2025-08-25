using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Models;

namespace MediaService.Persistence.Repositories;
public class OutboxEventConsumerRepository(IMongoDbContext context) : IOutboxEventConsumerRepository
{
    public async Task<bool> HasProcessedEventAsync(string eventId, string name, CancellationToken cancellationToken = default)
    {
        var builder = Builders<OutboxEventConsumer>.Filter;
        var filter = builder.And(
            builder.Eq(x => x.EventId, eventId),
            builder.Eq(x => x.Name, name)
        );
        return await context.OutboxEventConsumers.Find(filter).AnyAsync(cancellationToken); 
    }

    public async Task CreateEventAsync(IClientSessionHandle clientSession, OutboxEventConsumer eventConsumer, CancellationToken cancellationToken = default)
    {
        await context.OutboxEventConsumers.InsertOneAsync(clientSession, eventConsumer, null, cancellationToken: cancellationToken);
    }
}