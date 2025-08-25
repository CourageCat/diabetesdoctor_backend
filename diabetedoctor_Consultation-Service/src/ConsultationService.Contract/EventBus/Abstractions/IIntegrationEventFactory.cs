using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Abstractions;

public interface IIntegrationEventFactory
{
    IntegrationEvent? CreateEvent(string typeName, string value);
}