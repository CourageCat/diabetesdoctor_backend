using MediaService.Domain.Abstractions.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Domain.Models;

public class FavouriteCategory : DomainEntity<ObjectId>
{
    [BsonElement("category_id"), BsonRepresentation(BsonType.ObjectId)]
    public ObjectId CategoryId { get; private set; }
    [BsonElement("user_id")]
    public UserId UserId { get; private set; }

    public static FavouriteCategory Create(ObjectId id, ObjectId categoryId, UserId userId)
    {
        return new FavouriteCategory
        {
            Id = id,
            CategoryId = categoryId,
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false,
        };
    }
}
