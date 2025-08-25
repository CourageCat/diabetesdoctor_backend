namespace MediaService.Contract.Abstractions.Message;
public interface IDomainEvent : INotification
{
    public string Id { get; init; }
}
