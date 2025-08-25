using AuthService.Api.Features.Auth.Responses;
namespace AuthService.Api.Features.Auth.Commands;

public record LoginWithEmailCommand
    (string Email, string Password) : ICommand<Success<LoginResponse>>;