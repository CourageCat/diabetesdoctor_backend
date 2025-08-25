using AuthService.Api.Features.Auth.Responses;
namespace AuthService.Api.Features.Auth.Commands;

public record LoginWithPhoneNumberCommand
    (string PhoneNumber, string Password) : ICommand<Success<LoginResponse>>;