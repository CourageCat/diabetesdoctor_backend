namespace MediaService.Contract.EventBus.Abstractions.Message;

public interface IIntegrationEventHandler<in TEvent> : INotificationHandler<TEvent>
where TEvent : IntegrationEvent
{
}