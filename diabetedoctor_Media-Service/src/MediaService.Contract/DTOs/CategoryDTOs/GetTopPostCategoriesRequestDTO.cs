using MediaService.Contract.Enumarations.Post;

namespace MediaService.Contract.DTOs.CategoryDTOs;
public record GetTopPostCategoriesRequestDto
{
    public int NumberOfCategories { get; set; }
}
