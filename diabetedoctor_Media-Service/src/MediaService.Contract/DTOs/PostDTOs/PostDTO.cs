using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MediaService.Contract.DTOs.CategoryDTOs;
using MediaService.Contract.DTOs.UserDTOs;

namespace MediaService.Contract.DTOs.PostDTOs;
[BsonIgnoreExtraElements]
public record PostDto
{
    public PostDto(string id, string title, string thumbnail, DateTime createdDate, int view, int like, bool isBookMarked, bool isLiked, CategoryDto category, UserDto user)
    {
        Id = id;
        Title = title;
        Thumbnail = thumbnail;
        CreatedDate = createdDate;
        View = view;
        Like = like;
        IsBookMarked = isBookMarked;
        IsLiked = isLiked;
        Category = category;
        User = user;
    }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; init; }
    [BsonElement("title")]
    public string Title { get; init; }
    [BsonElement("thumbnail")]
    public string Thumbnail { get; init; }
    [BsonElement("created_date")]
    public DateTime CreatedDate { get; init; }
    [BsonElement("view")]
    public int View { get; init; }
    [BsonElement("like")]
    public int Like { get; init; }
    [BsonElement("is_bookmarked")]
    public bool IsBookMarked { get; init; }
    [BsonElement("is_liked")]
    public bool IsLiked { get; init; }
    [BsonElement("category")]
    public CategoryDto Category { get; init; }
    [BsonElement("user")]
    public UserDto User { get; init; }
}
