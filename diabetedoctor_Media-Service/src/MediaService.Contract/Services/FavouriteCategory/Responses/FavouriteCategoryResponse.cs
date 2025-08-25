using MediaService.Contract.DTOs.CategoryDTOs;
using MediaService.Contract.DTOs.PostDTOs;
using MongoDB.Bson.Serialization.Attributes;

namespace MediaService.Contract.Services.FavouriteCategory.Responses;

[BsonIgnoreExtraElements]
public record FavouriteCategoryResponse
{
    [BsonElement("category")]
    public CategoryDto Category { get; init; }
    [BsonElement("posts")]
    public List<PostDto> Posts { get; init; }

    public FavouriteCategoryResponse(CategoryDto category, List<PostDto> posts)
    {
        Category = category;
        Posts = posts;
    }
}