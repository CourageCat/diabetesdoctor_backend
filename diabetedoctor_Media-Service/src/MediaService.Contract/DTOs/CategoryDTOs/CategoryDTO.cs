using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediaService.Contract.DTOs.CategoryDTOs;
[BsonIgnoreExtraElements]
public record CategoryDto
{
    public CategoryDto(string id, string name, string imageUrl)
    {
        Id = id;
        Name = name;
        ImageUrl = imageUrl;
    }

    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
    [BsonElement("name")]
    public string Name { get; init; }
    [BsonElement("image_url")]
    public string ImageUrl { get; init; }
}
