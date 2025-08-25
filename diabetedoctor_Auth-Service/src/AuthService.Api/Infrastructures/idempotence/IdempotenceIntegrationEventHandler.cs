using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;

namespace AuthService.Api.Infrastructures.idempotence;

public sealed class IdempotenceIntegrationEventHandler<TIntegrationEvent>(
    ILogger<IdempotenceIntegrationEventHandler<TIntegrationEvent>> logger,
    IIntegrationEventHandler<TIntegrationEvent> decorated,
    IUnitOfWork unitOfWork,
    IRepositoryBase<OutboxEvent, Guid> eventRepository,
    IOutboxEventConsumerRepository consumerRepository)
    : IIntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent
{
    public async Task Handle(TIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var consumer = decorated.GetType().Name;
        if (await consumerRepository.HasProcessedEventAsync(notification.EventId.ToString(), consumer, cancellationToken))
        {
            return;
        }

        await decorated.Handle(notification, cancellationToken);
        var eventConsumer = OutboxEventConsumer.Create(notification.EventId.ToString(), consumer);
        consumerRepository.CreateEvent(eventConsumer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}