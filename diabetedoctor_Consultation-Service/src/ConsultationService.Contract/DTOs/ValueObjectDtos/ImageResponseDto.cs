using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Contract.DTOs.ValueObjectDtos;

[BsonIgnoreExtraElements]
public record ImageResponseDto
{
    [BsonElement("public_url")]
    public string PublicUrl { get; private init; } = null!;
}