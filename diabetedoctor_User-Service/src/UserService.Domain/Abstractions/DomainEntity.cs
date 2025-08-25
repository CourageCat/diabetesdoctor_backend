namespace UserService.Domain.Abstractions;

public class DomainEntity<TKey> : IDomainEntity
{
    public TKey Id { get; set; } = default!;
    public DateTime? CreatedDate { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    /// <summary>
    /// True if domain entity has an identity
    /// </summary>
    /// <returns></returns>
    public bool IsTransient()
    {
        return Id.Equals(default(TKey));
    }
}
