using AuthService.Api.Infrastructures.Abstractions.EventsBus;

namespace AuthService.Api.Infrastructures.Outbox;

internal sealed class OutboxProcessor(
    ApplicationDbContext context, 
    IIntegrationEventFactory  integrationEventFactory,
    IEventPublisher publisher)
{
    private const int BatchSize = 20;
    
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        var outboxMessages = await context.OutboxEvents
            .Where(e => e.ProcessedAt == null && e.RetryCount == 0)
            .OrderBy(e => e.CreatedDate)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);


        if (outboxMessages.Count == 0) return;
        
        foreach (var outboxMessage in outboxMessages)
        {
            try
            {
                var @event = integrationEventFactory.CreateEvent(outboxMessage.EventType, outboxMessage.Message);
                if (@event is null)
                    throw new InvalidOperationException($"EventType '{outboxMessage.EventType}' could not be deserialized.");

                await publisher.PublishAsync(outboxMessage.Topic, @event, outboxMessage.RetryCount, cancellationToken);

                // Update ProcessedAt
                outboxMessage.UpdateProcessedAt();
            }
            catch (Exception ex)
            {
                // Update ErrorMessage and ProcessedAt
                outboxMessage.UpdateErrorMessage(ex.ToString());
            }
            context.OutboxEvents.Update(outboxMessage);
            // Save all updates at once
            await context.SaveChangesAsync(cancellationToken);
        }
        
        await transaction.CommitAsync(cancellationToken);
    }
    
    public async Task ExecuteRetry(int retryCount, CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        
        var outboxMessages = await context.OutboxEvents
            .Where(e => e.ProcessedAt == null && e.RetryCount == retryCount)
            .OrderBy(e => e.CreatedDate)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (outboxMessages.Count == 0) return;

        foreach (var outboxMessage in outboxMessages)
        {
            // Skip if not visible yet
            if (outboxMessage.VisibleAt > DateTime.UtcNow) continue;

            try
            {
                var @event = integrationEventFactory.CreateEvent(outboxMessage.EventType, outboxMessage.Message);
                if (@event is null)
                    throw new InvalidOperationException($"EventType '{outboxMessage.EventType}' could not be deserialized.");

                await publisher.PublishAsync(outboxMessage.Topic, @event, outboxMessage.RetryCount, cancellationToken);
                
                // Update ProcessedAt
                outboxMessage.UpdateProcessedAt();
            }
            catch (Exception ex)
            {
                // Update ErrorMessage and ProcessedAt
                outboxMessage.UpdateErrorMessage(ex.ToString());
            }

            // Optional: explicitly mark the entity as modified
            context.OutboxEvents.Update(outboxMessage);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        await transaction.CommitAsync(cancellationToken);
    }
}