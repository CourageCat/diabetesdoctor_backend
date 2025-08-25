using MongoDB.Bson.Serialization.Attributes;

namespace ChatService.Contract.DTOs.UserDtos;

[BsonIgnoreExtraElements]
public record UserResponseDto
{
    [BsonElement("_id")]
    public string Id { get; init; } = null!;
    
    [BsonElement("avatar")]
    public string? Avatar { get; init; } = null!;
    
    [BsonElement("full_name")]
    public string? FullName { get; init; } = null!;
    
    [BsonElement("phone_number")]
    public string? PhoneNumber { get; init; }
    
    [BsonElement("email")]
    public string? Email { get; init; }
 
    [BsonElement("status")]
    public int? Status { get; init; }
    
    [BsonElement("role")]
    public int Role { get; init; }
}