
using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Domain.Models;

public class Conversation : DomainEntity<ObjectId>
{
    [BsonElement("conversation_id")]
    public ConversationId ConversationId { get; private set; } = null!;

    [BsonElement("name")]
    public string Name { get; private set; } = null!;

    [BsonElement("avatar")]
    public Image Avatar { get; private set; } = null!;

    [BsonElement("members")] 
    public List<UserId> Members { get; private init; } = [];

    public static Conversation CreateGroup(ObjectId id, ConversationId conversationId, string name, Image avatar, List<UserId> members)
    {
        return new Conversation
        {
            Id = id,
            ConversationId = conversationId,
            Name = name,
            Avatar = avatar,
            Members = members,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
    }
    
    //public void AddMembers(List<UserId> members)
    //{
    //    Changes["members"] = members;
    //}
}