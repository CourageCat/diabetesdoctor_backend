using NotificationService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Abstractions.Repositories
{
    public interface IOutboxEventConsumerRepository
    {
        Task<bool> HasProcessedEventAsync(string eventId, string name, CancellationToken cancellationToken = default);
        Task CreateEventAsync(OutboxEventConsumer eventConsumer, CancellationToken cancellationToken = default);
    }
}
