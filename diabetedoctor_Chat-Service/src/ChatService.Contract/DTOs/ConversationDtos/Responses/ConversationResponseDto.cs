using ChatService.Contract.DTOs.MessageDtos;
using ChatService.Contract.DTOs.ParticipantDtos;
using MongoDB.Bson.Serialization.Attributes;

namespace ChatService.Contract.DTOs.ConversationDtos.Responses;


[BsonIgnoreExtraElements]
public record ConversationResponseDto
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    
    [BsonElement("other_user_id")]
    public string? OtherUserId { get; init; }
    
    [BsonElement("name")]
    public string Name { get; init; } = null!;
    
    [BsonElement("avatar")]
    public string Avatar { get; init; } = null!;
    
    [BsonElement("type")]
    public int ConversationType { get; init; }
    
    [BsonElement("member_count")]
    public long MemberCount { get; init; }
    
    [BsonElement("status")]
    public int Status { get; init; }
    
    [BsonElement("can_view")]
    public bool CanView { get; init; }
    
    [BsonElement("modified_date")]
    public DateTime ModifiedDate { get; init; }
    
    [BsonElement("last_message")]
    public MessageResponseDto? LastMessage { get; init; }
}
