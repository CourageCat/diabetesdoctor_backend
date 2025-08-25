namespace AuthService.Api.Persistences.Abstractions;

public interface IAggregateRoot<TId> : IAggregateRoot, IDomainEntity<TId>
{
}

public interface IAggregateRoot : IDomainEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    IDomainEvent[] ClearDomainEvents();
}
