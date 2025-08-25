using MediaService.Domain.Abstractions.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Domain.Models;

public class PostCategory : DomainEntity<ObjectId>
{
    [BsonElement("post_id"), BsonRepresentation(BsonType.ObjectId)]
    public ObjectId PostId { get; private set; }

    // đổi thành value object
    [BsonElement("category_id"), BsonRepresentation(BsonType.ObjectId)]
    public ObjectId CategoryId { get; private set; }

    public static PostCategory Create(ObjectId id, ObjectId postId, ObjectId categoryId)
    {
        return new PostCategory
        {
            Id = id,
            PostId = postId,
            CategoryId = categoryId,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}
