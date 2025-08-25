using ConsultationService.Contract.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Contract.DTOs.ConsultationTemplateDtos.Responses;

[BsonIgnoreExtraElements]
public record ConsultationTemplateResponseDto
{
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = null!;
    
    [BsonElement("start_time")]
    public string StartTime { get; init; } = null!;
    
    [BsonElement("end_time")]
    public string EndTime { get; init; } = null!;
    
    [BsonElement("status")]
    public ConsultationTemplateStatusEnum Status { get; init; }
};