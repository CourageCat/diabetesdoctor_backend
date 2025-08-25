using AuthService.Api.Features.Auth.Responses;

namespace AuthService.Api.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : ICommand<Success<LoginResponse>>;