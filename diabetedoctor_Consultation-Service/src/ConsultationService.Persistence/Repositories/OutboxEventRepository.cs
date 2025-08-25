using ConsultationService.Domain.Abstractions;
using ConsultationService.Domain.Abstractions.Repositories;
using ConsultationService.Domain.Models;

namespace ConsultationService.Persistence.Repositories;

public class OutboxEventRepository(IMongoDbContext context) : RepositoryBase<OutboxEvent>(context), IOutboxEventRepository
{
    public async Task SaveAsync(OutboxEvent @event, CancellationToken cancellationToken = default)
    {
        await DbSet.InsertOneAsync(@event, cancellationToken: cancellationToken);
    }
}