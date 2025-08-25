namespace AuthService.Api.Features.Auth.Commands;

public record ResetPasswordCommand : ICommand<Success>
{
    public string PhoneNumber { get; init; } = string.Empty;
    public string Otp { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}