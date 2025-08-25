namespace AuthService.Api.Features.Auth.Commands;

public record SendOtpChangePasswordCommand : ICommand<Success>
{
    public Guid UserId { get; init; }
}