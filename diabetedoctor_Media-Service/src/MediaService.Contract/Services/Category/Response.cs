using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Contract.Services.Category;

[BsonIgnoreExtraElements]
public record CategoryResponse
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
    [BsonElement("name")]
    public string Name { get; init; }
    [BsonElement("description")]
    public string Description { get; init; }
    [BsonElement("image_url")]
    public string ImageUrl { get; init; }
    [BsonElement("created_date")]
    public DateTime CreatedDate { get; init; }
    [BsonElement("is_added_to_favourite")]
    public bool? IsAddedToFavourite { get; init; }
    [BsonElement("number_of_posts")]
    public int? NumberOfPosts { get; init; }
}
