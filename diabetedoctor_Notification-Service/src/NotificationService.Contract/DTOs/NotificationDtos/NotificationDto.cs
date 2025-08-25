using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Contract.DTOs.NotificationDtos;

[BsonIgnoreExtraElements]
public record NotificationDto
{
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("user_id")]
    public string? UserId { get; set; } = string.Empty;
    [BsonElement("type")]
    public int Type { get; set; }
    [BsonElement("is_read")]
    public bool IsRead { get; set; }
    [BsonElement("read_at")]
    public DateTime? ReadAt { get; set; } = default;
    [BsonElement("received_at")]
    public DateTime? ReceivedAt { get; set; } = default;
    [BsonElement("payload")]
    public object? Payload { get; set; }
}