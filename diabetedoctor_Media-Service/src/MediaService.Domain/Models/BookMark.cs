using MediaService.Domain.Abstractions.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Domain.Models;

public class BookMark : DomainEntity<ObjectId>
{
    [BsonElement("post_id"), BsonRepresentation(BsonType.ObjectId)]
    public ObjectId PostId { get; private set; }
    
    // đổi thành value object
    [BsonElement("user_id")]
    public UserId UserId { get; private set; }

    public static BookMark Create(ObjectId id, ObjectId postId, UserId userId)
    {
        return new BookMark
        {
            Id = id,
            PostId = postId,
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}
