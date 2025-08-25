namespace AuthService.Api.Persistences.Abstractions;
public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
