using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Contract.Enums;

namespace NotificationService.Contract.DTOs.ValueObjectDtos;

[BsonIgnoreExtraElements]
public record FileAttachmentDto
{
    [BsonElement("public_id")]
    public string PublicId { get; init; } = null!;
    
    [BsonElement("public_url")]
    public string PublicUrl { get; init; } = null!;
    
    [BsonElement("file_type")]
    public MediaTypeEnum Type { get; init; }
}