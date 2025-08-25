using MongoDB.Bson;

namespace MediaService.Contract.DTOs.BookMarkDTOs;
public record GetAllPostsFromBookMarkRequestDto
{
    public string? SearchContent { get; set; }
    public ObjectId? CategoryId { get; set; }
    public string? UserCreatedId { get; set; }
}
