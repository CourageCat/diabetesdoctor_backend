using MongoDB.Bson.Serialization.Attributes;

namespace ChatService.Contract.DTOs.ParticipantDtos;

[BsonIgnoreExtraElements]
public record ParticipantResponseDto
{
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; init; }
    
    [BsonElement("conversation_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ConversationId { get; init; }
    
    [BsonElement("full_name")]
    public string FullName { get; init; } = null!;
    
    [BsonElement("avatar")]
    public string Avatar { get; init; } = null!;
    
    [BsonElement("phone_number")]
    public string? PhoneNumber { get; init; }
    
    [BsonElement("email")]
    public string? Email { get; init; }
    
    [BsonElement("role")]
    public int Role { get; init; }
    
    [BsonElement("status")]
    public int? Status { get; init; } 
    
    [BsonElement("invited_by")]
    public string InvitedBy { get; init; } = null!;
}