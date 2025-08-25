namespace AuthService.Api.Persistences.Abstractions;

public interface IDomainEntity<T> : IDomainEntity
{
    public T Id { get; set; }
}

public interface IDomainEntity
{
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}