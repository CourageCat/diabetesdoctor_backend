namespace NotificationService.Contract.EventBus.Abstractions;

public interface IIntegrationEventFactory
{
    IntegrationEvent? CreateEvent(string typeName, string value);
}