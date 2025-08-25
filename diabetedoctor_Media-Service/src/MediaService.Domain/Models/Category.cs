using MediaService.Domain.Abstractions.Entities;

using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Domain.Models;

public class Category : DomainEntity<ObjectId>
{
    [BsonElement("name")]
    public string Name { get; private set; } = default!;
    [BsonElement("description")]
    public string Description { get; private set; } = default!;
    [BsonElement("image")]
    public Image Image { get; private set; } = default!;

    public static Category Create(ObjectId id, string name, string description, Image image)
    {
        return new Category
        {
            Id = id,
            Name = name,
            Description = description,
            Image = image,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public void Update(string name, string description, Image image)
    {
        Name = name;
        Description = description;
        Image = image;
        ModifiedDate = DateTime.UtcNow;
    }
}