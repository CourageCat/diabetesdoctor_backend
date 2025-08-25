namespace UserService.Contract.Domain;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{
}
