namespace AuthService.Api.Features.Auth.Commands;

public record ChangePasswordCommand : ICommand<Success>
{
    public Guid UserId { get; init; }
    public string Otp { get; init; } = string.Empty;
    public string OldPassword { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}