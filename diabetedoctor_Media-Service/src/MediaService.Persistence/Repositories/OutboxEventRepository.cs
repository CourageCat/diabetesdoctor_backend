using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Models;

namespace MediaService.Persistence.Repositories;
public class OutboxEventRepository(IMongoDbContext context) : RepositoryBase<OutboxEvent>(context), IOutboxEventRepository
{
    public async Task SaveAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default)
    {
        await DbSet.InsertOneAsync(outboxEvent, cancellationToken: cancellationToken);
    }
}
