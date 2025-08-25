using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.Enums;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Models;

public class Notification : DomainEntity<ObjectId>
{
    [BsonElement("user_id")]
    public UserId UserId { get; private set; } = default!;

    [BsonRepresentation(BsonType.Boolean)]
    [BsonElement("is_read")]
    public bool IsRead { get; private set; }

    [BsonElement("notification_type")]
    public NotificationType Type { get; private set; } = default!; 

    [BsonElement("read_at")]
    public DateTime? ReadAt { get; private set; }

    [BsonElement("received_at")]
    public DateTime? ReceivedAt { get; private set; }

    [BsonElement("payload")]
    public BsonDocument? Payload { get; private set; }

    public static IEnumerable<Notification> CreateManyNotification(IEnumerable<UserId> userIds, NotificationType type, BsonDocument payload)
    {
        return userIds.Select(userId => new Notification
        {
            UserId = userId,
            IsRead = false,
            Type = type,
            ReadAt = null,
            ReceivedAt = DateTime.Now,
            Payload = payload,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
            IsDeleted = false
        }).ToList();
    }

}