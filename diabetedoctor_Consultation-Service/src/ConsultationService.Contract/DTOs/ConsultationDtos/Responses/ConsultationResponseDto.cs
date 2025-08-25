using ConsultationService.Contract.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Contract.DTOs.ConsultationDtos.Responses;

public record ConsultationResponseDto
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = null!;
    [BsonElement("date")]
    public DateOnly Date { get; init; }
    [BsonElement("start_time")]
    public string StartTime { get; init; } = null!;
    [BsonElement("end_time")]
    public string EndTime { get; init; } = null!;
    [BsonElement("status")]
    public ConsultationStatusEnum Status { get; init; }
    [BsonElement("user_full_name")]
    public string UserFullName { get; init; } = null!;
    [BsonElement("user_avatar")]
    public string UserAvatar { get; init; } = null!;
    [BsonElement("price")] 
    public double? Price { get; init; }
    [BsonElement("conversation_id")] 
    public string? ConversationId { get; init; }
    [BsonElement("reason")]
    public string? Reason { get; init; }
}