namespace UserService.Domain.Models;

public sealed class ModeratorProfile : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public UserInfo User { get; private set; } = null!;
    
    private ModeratorProfile(
        Guid id,
        Guid userId)
    {
        Id = id;
        UserId = userId;
        CreatedDate = DateTime.UtcNow;
        ModifiedDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    // Tạo thông tin
    public static ModeratorProfile Create
    (Guid id, 
        Guid userId)
    {
        var moderatorProfile =  new ModeratorProfile(id, userId);
        return moderatorProfile;
    }
}