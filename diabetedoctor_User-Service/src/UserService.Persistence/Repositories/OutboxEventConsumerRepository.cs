namespace UserService.Persistence.Repositories;

public class OutboxEventConsumerRepository(ApplicationDbContext context) : IOutboxEventConsumerRepository
{
    public async Task<bool> HasProcessedEventAsync(string eventId, string name,
        CancellationToken cancellationToken = default)
    {
        IQueryable<OutboxEventConsumer> items = context.Set<OutboxEventConsumer>().AsNoTracking();
        items = items.Where(x => x.EventId == eventId && x.Name == name);
        return await items.AnyAsync(cancellationToken);
    }

    public void CreateEvent(OutboxEventConsumer eventConsumer)
    {
        context.OutboxEventConsumers.Add(eventConsumer);
    }
}