using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Contract.DTOs.ValueObjectDtos;

public record UserIdDto
{
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; init; } = null!;
}