namespace UserService.Contract.Domain;

public interface IDomainEntity<T> : IDomainEntity
{
    public T Id { get; set; }
}

public interface IDomainEntity
{
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}