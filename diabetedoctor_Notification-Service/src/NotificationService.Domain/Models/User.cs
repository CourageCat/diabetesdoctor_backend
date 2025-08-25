using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Contract.Helpers;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Models;

public class User : DomainEntity<ObjectId>
{
    [BsonElement("user_id")]
    public UserId UserId { get; private set; } = null!;
    [BsonElement("full_name")]
    public string FullName { get; private set; } = null!;
    [BsonElement("avatar")]
    public Image Avatar { get; private set; } = null!;
    [BsonElement("fcm_token")]
    public string? FcmToken { get; private set; }

    public static User Create(ObjectId id, UserId userId, string fullName, Image avatar, string? fcmToken = null)
    {
        return new User
        {
            Id = id,
            UserId = userId,
            FullName = fullName,
            Avatar = avatar,
            FcmToken = fcmToken,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsDeleted = false
        };
    }
    
    public void Modify(string? fullname, Image? avatar)
    {
        FullName = fullname ?? FullName;
        Avatar = avatar ?? Avatar;
        ModifiedDate = CurrentTimeService.GetCurrentTime();
    }
}