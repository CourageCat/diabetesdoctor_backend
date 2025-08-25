namespace UserService.Contract.Services.Moderators.Responses;

public record ModeratorResponse
{
    public string Id { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string Name { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public GenderEnum Gender { get; init; }
    public DateTime? CreatedDate { get; init; }
}