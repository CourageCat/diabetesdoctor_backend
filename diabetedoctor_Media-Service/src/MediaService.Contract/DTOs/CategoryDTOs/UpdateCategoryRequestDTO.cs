using MongoDB.Bson;

namespace MediaService.Contract.DTOs.CategoryDTOs;

public record UpdateCategoryRequestDto
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public ObjectId Image { get; set; } = default!;
}
