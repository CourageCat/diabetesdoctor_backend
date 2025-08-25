using MediaService.Domain.Abstractions.Entities;
using MediaService.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Domain.Models;

public class Media : DomainEntity<ObjectId>
{

    [BsonElement("public_id")] 
    public string PublicId { get; private set; } = null!;
    [BsonElement("public_url")]
    public string PublicUrl { get; private set; } = null!;
    [BsonElement("uploaded_by")]
    public UserId UploadedBy { get; private set; } = null!;
    [BsonElement("is_used")]
    public bool IsUsed { get; private set; }
    [BsonElement("expired_at")]
    public DateTime? ExpiredAt { get; private set; }
    [BsonElement("type")]
    public MediaType Type { get; private set; }

    public static Media Create(ObjectId id, string publicId, string publicUrl, MediaType type, UserId uploadedBy)
    {
        return new Media
        {
            Id = id,
            PublicId = publicId,
            PublicUrl = publicUrl,
            Type = type,
            UploadedBy = uploadedBy,
            IsUsed = false,
            ExpiredAt = DateTime.UtcNow.AddHours(1),
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }
    
    public static Media CreateForSeedData(ObjectId id, string publicId, string publicUrl, MediaType type, UserId uploadedBy)
    {
        return new Media
        {
            Id = id,
            PublicId = publicId,
            PublicUrl = publicUrl,
            Type = type,
            UploadedBy = uploadedBy,
            IsUsed = true,
            ExpiredAt = null,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public void Used()
    {
        IsUsed = true;
        ExpiredAt = null;
    }
}