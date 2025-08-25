using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;

namespace AuthService.Api.Infrastructures.Abstractions.EventsBus;

public interface IIntegrationEventFactory
{
    IntegrationEvent? CreateEvent(string typeName, string value);
}