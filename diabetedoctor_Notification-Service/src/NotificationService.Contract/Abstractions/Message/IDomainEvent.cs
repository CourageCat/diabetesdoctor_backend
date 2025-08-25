namespace NotificationService.Contract.Abstractions.Message;

public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
