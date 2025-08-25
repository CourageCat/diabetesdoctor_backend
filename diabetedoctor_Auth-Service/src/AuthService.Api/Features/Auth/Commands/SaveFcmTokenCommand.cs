namespace AuthService.Api.Features.Auth.Commands;

public record SaveFcmTokenCommand : ICommand<Success>
{
    public Guid UserId { get; init; }
    public string FcmToken { get; init; } = null!;
}