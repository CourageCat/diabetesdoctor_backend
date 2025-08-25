using MediaService.Contract.Enumarations.Post;
using MongoDB.Bson;

namespace MediaService.Contract.DTOs.CategoryDTOs;
public record GetAllCategoriesRequestDto
{
    public string? SearchContent { get; set; }
}
