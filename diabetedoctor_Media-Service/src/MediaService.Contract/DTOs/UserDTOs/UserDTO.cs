using MediaService.Contract.DTOs.MediaDTOs;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MediaService.Contract.DTOs.UserDTOs;

[BsonIgnoreExtraElements]
public record UserDto
{
    public UserDto(string id, string fullName, string imageUrl)
    {
        Id = id;
        FullName = fullName;
        ImageUrl = imageUrl;
    }

    [BsonElement("_id")]
    public string Id { get; init; }
    [BsonElement("full_name")]
    public string FullName { get; init; }
    [BsonElement("public_url")]
    public string ImageUrl { get; init; }
}

