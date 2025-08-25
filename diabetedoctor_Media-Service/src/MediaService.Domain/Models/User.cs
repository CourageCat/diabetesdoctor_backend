using MediaService.Contract.Enumarations.User;
using MediaService.Domain.Abstractions.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Domain.Models;

public class User : DomainEntity<ObjectId>
{

    [BsonElement("full_name")]
    public string FullName { get; private set; }
    [BsonElement("avatar")]
    public Image Avatar { get; private set; }
    [BsonElement("user_id")]
    public UserId UserId { get; private set; }
    [BsonElement("role")]
    public RoleType Role { get; private set; }

    public static User Create(ObjectId id, string fullName, Image avatar, UserId userId, RoleType role)
    {
        return new User
        {
            Id = id,
            FullName = fullName,
            Avatar = avatar,
            UserId = userId,
            Role = role,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}