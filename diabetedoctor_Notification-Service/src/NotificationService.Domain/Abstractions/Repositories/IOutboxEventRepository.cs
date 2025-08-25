using NotificationService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Abstractions.Repositories
{
    public interface IOutboxEventRepository : IRepositoryBase<OutboxEvent>
    {
        Task SaveAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default);
    }
}
