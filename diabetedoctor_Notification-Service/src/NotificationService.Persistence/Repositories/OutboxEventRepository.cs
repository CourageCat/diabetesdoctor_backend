using NotificationService.Domain;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.Abstractions.Repositories;
using NotificationService.Domain.Models;

namespace NotificationService.Persistence.Repositories
{
    public class OutboxEventRepository(IMongoDbContext context) : RepositoryBase<OutboxEvent>(context), IOutboxEventRepository
    {
        public async Task SaveAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default)
        {
            await DbSet.InsertOneAsync(outboxEvent, cancellationToken: cancellationToken);
        }
    }
}
