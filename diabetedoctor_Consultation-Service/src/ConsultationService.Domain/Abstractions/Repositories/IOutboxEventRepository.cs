using ConsultationService.Domain.Models;

namespace ConsultationService.Domain.Abstractions.Repositories;

public interface IOutboxEventRepository : IRepositoryBase<OutboxEvent>
{
    Task SaveAsync(OutboxEvent @event, CancellationToken cancellationToken = default);
}