using MediaService.Contract.Enumarations.Post;
using MongoDB.Bson;

namespace MediaService.Contract.DTOs.PostDTOs.GetAllPostsRequestDTO;
public record GetAllPostsByUserRequestDto
{
    public string? SearchContent { get; set; }
    public ObjectId[]? CategoryIds { get; set; } = default!;
    public string? DoctorId { get; set; }
    public string? ModeratorId { get; set; }
    public TutorialEnum? TutorialType { get; set; }
}
