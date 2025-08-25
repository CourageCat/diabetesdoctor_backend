using MediaService.Contract.Enumarations.Post;
using MongoDB.Bson;

namespace MediaService.Contract.DTOs.PostDTOs.GetAllPostsRequestDTO;
public record GetAllPostsBySystemRequestDto
{
    public string? SearchContent { get; set; }
    public ObjectId[]? CategoryIds { get; set; } = default!;
    public Status? Status { get; set; }
    public string? ModeratorId { get; set; }
    public string? DoctorId { get; set; }
}
