namespace AuthService.Api.Features.Auth.Commands;

public record LogoutCommand(Guid UserId) : ICommand<Success>;
