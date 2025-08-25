using MediaService.Contract.DTOs.CategoryDTOs;
using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.DTOs.UserDTOs;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using MediaService.Contract.Enumarations.Post;
using static MediaService.Contract.DTOs.PostDTOs.PostDto;

namespace MediaService.Contract.Services.Post;

[BsonIgnoreExtraElements]
public record PostResponse
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = default!;

    [BsonElement("title")] 
    public string Title { get; init; } = default!;
    [BsonElement("content")]
    public string? Content { get; init; }
    [BsonElement("content_html")]
    public string? ContentHtml { get; init; }
    [BsonElement("thumbnail")]
    public string Thumbnail { get; init; } = default!;
    [BsonElement("created_date")]
    public DateTime CreatedDate { get; init; }
    [BsonElement("modified_date")]
    public DateTime? ModifiedDate { get; init; }
    [BsonElement("view")]
    public int View { get; init; }
    [BsonElement("like")]
    public int Like { get; init; }
    [BsonElement("categories")]
    public List<CategoryDto> Categories { get; init; }
    [BsonElement("status")]
    public Status Status { get; init; }
    [BsonElement("reason_rejected")]
    public string? ReasonRejected { get; init; }
    [BsonElement("moderator")] 
    public UserDto Moderator { get; init; } = default!;
    [BsonElement("doctor")] 
    public UserDto Doctor { get; init; } = default!;
    [BsonElement("is_bookmarked")]
    public bool? IsBookMarked { get; init; }
    [BsonElement("is_liked")]
    public bool? IsLiked { get; init; }
    [BsonElement("word_count")]
    public long? WordCount { get; init; }
    [BsonElement("bookmarked_date")]
    public DateTime? BookmarkedDate { get; init; }
    [BsonElement("liked_date")]
    public DateTime? LikedDate { get; init; }
}

[BsonIgnoreExtraElements]
public record PostCreatedResponse
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; }
}


