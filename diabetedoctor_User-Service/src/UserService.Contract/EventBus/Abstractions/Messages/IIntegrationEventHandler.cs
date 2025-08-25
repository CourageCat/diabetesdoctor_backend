namespace UserService.Contract.EventBus.Abstractions.Messages;

public interface IIntegrationEventHandler<in TEvent> : INotificationHandler<TEvent>
where TEvent : IntegrationEvent
{
}