using MediaService.Contract.Enumarations.Post;

namespace MediaService.Contract.DTOs.PostDTOs;
public record GetTopViewPostsRequestDto
{
    public int NumberOfPosts { get; set; }
    public int NumberOfDays { get; set; }
}
