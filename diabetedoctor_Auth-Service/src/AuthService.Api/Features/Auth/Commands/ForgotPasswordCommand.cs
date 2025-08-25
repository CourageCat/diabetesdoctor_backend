namespace AuthService.Api.Features.Auth.Commands;

public record ForgotPasswordCommand : ICommand<Success>
{
    public string PhoneNumber { get; init; } = string.Empty;
}