namespace UserService.Contract.DTOs;

public record ImageResponseDto
{
    public string Id { get; init; }
    public string ImageUrl { get; init; }
}