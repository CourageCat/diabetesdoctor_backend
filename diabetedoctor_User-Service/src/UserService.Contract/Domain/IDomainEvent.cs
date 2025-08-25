namespace UserService.Contract.Domain;
public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
