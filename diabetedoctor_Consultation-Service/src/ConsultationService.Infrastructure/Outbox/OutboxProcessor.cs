using ConsultationService.Contract.EventBus.Abstractions;
using ConsultationService.Contract.Helpers;
using ConsultationService.Domain.Abstractions;
using ConsultationService.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsultationService.Infrastructure.Outbox;

internal sealed class OutboxProcessor(
    IUnitOfWork unitOfWork, 
    IMongoDbContext mongoDbContext, 
    IIntegrationEventFactory  integrationEventFactory,
    IEventPublisher publisher)
{
    private const int BatchSize = 20;
    
    public async Task Execute(CancellationToken cancellationToken = default)
    {
        await unitOfWork.StartTransactionAsync(cancellationToken);
        
        var outboxMessages = await mongoDbContext.OutboxEvents
            .Find(@event => @event.ProcessedAt == null && @event.RetryCount == 0)
            .Sort(Builders<OutboxEvent>.Sort.Ascending(x => x.CreatedDate))
            .Limit(BatchSize)
            .ToListAsync(cancellationToken);

        if (outboxMessages.Count == 0) return;
        
        foreach (var outboxMessage in outboxMessages)
        {
            var filter = Builders<OutboxEvent>.Filter.Eq(outboxEvent => outboxEvent.Id, outboxMessage.Id);
            var update = Builders<OutboxEvent>.Update.Set(outboxEvent => outboxEvent.ProcessedAt, CurrentTimeService.GetCurrentTimeUtc());
            var option = new UpdateOptions { IsUpsert = false };
            
            try
            {
                var @event = integrationEventFactory.CreateEvent(outboxMessage.EventType, outboxMessage.Message);
                if (@event is null) throw new InvalidOperationException($"EventType '{outboxMessage.EventType}' could not be deserialized.");
                await publisher.PublishAsync(outboxMessage.Topic, @event, outboxMessage.RetryCount,cancellationToken);
                await mongoDbContext.OutboxEvents.UpdateOneAsync(unitOfWork.ClientSession, filter, update, option, cancellationToken);
            }
            catch (Exception ex)
            {
                var updateError = Builders<OutboxEvent>.Update.Set(outboxEvent => outboxEvent.ErrorMessage, ex.ToString());
                var updateWithError = Builders<OutboxEvent>.Update.Combine(update, updateError);
                await mongoDbContext.OutboxEvents.UpdateOneAsync(unitOfWork.ClientSession, filter, updateWithError, option, cancellationToken);
            }
        }

        await unitOfWork.CommitTransactionAsync(cancellationToken);
    }
    
    public async Task ExecuteRetry(int retryCount, CancellationToken cancellationToken = default)
    {
        await unitOfWork.StartTransactionAsync(cancellationToken);
        
        var outboxMessages = await mongoDbContext.OutboxEvents
            .Find(@event => @event.ProcessedAt == null && @event.RetryCount > retryCount)
            .Sort(Builders<OutboxEvent>.Sort.Ascending(x => x.CreatedDate))
            .Limit(BatchSize)
            .ToListAsync(cancellationToken);

        if (outboxMessages.Count == 0) return;
        
        foreach (var outboxMessage in outboxMessages)
        {
            if (outboxMessage.VisibleAt > CurrentTimeService.GetCurrentTimeUtc()) continue; 
            var filter = Builders<OutboxEvent>.Filter.Eq(outboxEvent => outboxEvent.Id, outboxMessage.Id);
            var update = Builders<OutboxEvent>.Update.Set(outboxEvent => outboxEvent.ProcessedAt, CurrentTimeService.GetCurrentTimeUtc());
            var option = new UpdateOptions { IsUpsert = false };
            
            try
            {
                var @event = integrationEventFactory.CreateEvent(outboxMessage.EventType, outboxMessage.Message);
                if (@event is null) throw new InvalidOperationException($"EventType '{outboxMessage.EventType}' could not be deserialized.");
                await publisher.PublishAsync(outboxMessage.Topic, @event, outboxMessage.RetryCount,cancellationToken);
                await mongoDbContext.OutboxEvents.UpdateOneAsync(unitOfWork.ClientSession, filter, update, option, cancellationToken);
            }
            catch (Exception ex)
            {
                var updateError = Builders<OutboxEvent>.Update.Set(outboxEvent => outboxEvent.ErrorMessage, ex.ToString());
                var updateWithError = Builders<OutboxEvent>.Update.Combine(update, updateError);
                await mongoDbContext.OutboxEvents.UpdateOneAsync(unitOfWork.ClientSession, filter, updateWithError, option, cancellationToken);
            }
        }

        await unitOfWork.CommitTransactionAsync(cancellationToken);
    }
    
    public async Task ExecuteSchedule(CancellationToken cancellationToken = default)
    {
        await unitOfWork.StartTransactionAsync(cancellationToken);
        
        var outboxScheduleEvents = await mongoDbContext.OutboxScheduleEvents
            .Find(@event => @event.IsCreated == false)
            .Sort(Builders<OutboxScheduleEvent>.Sort.Ascending(x => x.VisibleAt))
            .Limit(BatchSize)
            .ToListAsync(cancellationToken);

        if (outboxScheduleEvents.Count == 0) return;
        
        foreach (var outboxScheduleEvent in outboxScheduleEvents)
        {
            if (CurrentTimeService.GetVietNamCurrentTime() < outboxScheduleEvent.VisibleAt) continue; 
            var filter = Builders<OutboxScheduleEvent>.Filter.Eq(@event => @event.Id, outboxScheduleEvent.Id);
            var update = Builders<OutboxScheduleEvent>.Update.Set(@event => @event.IsCreated, true);
            var option = new UpdateOptions { IsUpsert = false };
            try
            {
                var outboxEvent = OutboxEvent.Create(ObjectId.GenerateNewId(), outboxScheduleEvent.Topic, outboxScheduleEvent.EventType, outboxScheduleEvent.Message, 0, 0);
                await mongoDbContext.OutboxScheduleEvents.UpdateOneAsync(unitOfWork.ClientSession, filter, update, option, cancellationToken);
                await mongoDbContext.OutboxEvents.InsertOneAsync(unitOfWork.ClientSession, outboxEvent, null, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        await unitOfWork.CommitTransactionAsync(cancellationToken);
    }

}