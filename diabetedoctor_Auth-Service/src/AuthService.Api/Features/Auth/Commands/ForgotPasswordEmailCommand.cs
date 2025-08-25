namespace AuthService.Api.Features.Auth.Commands;

public record ForgotPasswordEmailCommand : ICommand<Success>
{
    public string Email { get; init; } = string.Empty;
}