
using MediaService.Contract.Enumarations.Media;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Contract.DTOs.ValueObjectDtos;

[BsonIgnoreExtraElements]
public record FileAttachmentDto
{
    [BsonElement("public_id")]
    public string PublicId { get; init; } = null!;
    
    [BsonElement("public_url")]
    public string PublicUrl { get; init; } = null!;
    
    [BsonElement("file_type")]
    public MediaType Type { get; init; }
}