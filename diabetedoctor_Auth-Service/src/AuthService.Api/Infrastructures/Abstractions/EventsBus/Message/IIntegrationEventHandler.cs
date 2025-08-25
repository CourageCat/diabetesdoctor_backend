namespace AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;

public interface IIntegrationEventHandler<in TEvent> : INotificationHandler<TEvent>
where TEvent : IntegrationEvent
{
}