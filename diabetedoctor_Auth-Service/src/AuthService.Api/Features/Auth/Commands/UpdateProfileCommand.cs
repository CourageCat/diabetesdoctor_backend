namespace AuthService.Api.Features.Auth.Commands;

public record UpdateProfileCommand : ICommand<Success>
{
    public Guid UserId { get; init; }
    public string? FullName { get; init; }
    public string? Avatar { get; init; } = null!;
}