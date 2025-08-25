using MediaService.Contract.EventBus.Abstractions.Message;

namespace MediaService.Contract.EventBus.Abstractions;

public interface IIntegrationEventFactory
{
    IntegrationEvent? CreateEvent(string typeName, string value);
}