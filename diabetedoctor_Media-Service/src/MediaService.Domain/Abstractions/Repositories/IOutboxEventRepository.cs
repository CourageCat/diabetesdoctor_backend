using MediaService.Domain.Models;

namespace MediaService.Domain.Abstractions.Repositories;
public interface IOutboxEventRepository : IRepositoryBase<OutboxEvent>
{
    Task SaveAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default);
}
