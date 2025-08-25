using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Contract.DTOs.ValueObjectDtos;

[BsonIgnoreExtraElements]
public record ImageResponseDto
{
    [BsonElement("public_url")]
    public string PublicUrl { get; private init; } = null!;
}