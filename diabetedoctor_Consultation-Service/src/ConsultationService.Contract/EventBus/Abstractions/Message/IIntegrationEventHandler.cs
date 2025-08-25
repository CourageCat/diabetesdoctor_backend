using MediatR;

namespace ConsultationService.Contract.EventBus.Abstractions.Message;

public interface IIntegrationEventHandler<in TEvent> : INotificationHandler<TEvent>
where TEvent : IntegrationEvent
{
}