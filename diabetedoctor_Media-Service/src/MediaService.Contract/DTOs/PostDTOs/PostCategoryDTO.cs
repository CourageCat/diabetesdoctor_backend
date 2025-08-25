using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MediaService.Contract.DTOs.CategoryDTOs;
using MediaService.Contract.DTOs.UserDTOs;

namespace MediaService.Contract.DTOs.PostDTOs;
[BsonIgnoreExtraElements]
public record PostCategoryDto
{
    public PostCategoryDto(CategoryDto category)
    {
        Category = category;
    }
    [BsonElement("category")]
    public CategoryDto Category { get; init; }
}
