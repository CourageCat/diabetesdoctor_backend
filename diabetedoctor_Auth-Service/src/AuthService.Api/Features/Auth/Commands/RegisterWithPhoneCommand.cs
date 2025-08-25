namespace AuthService.Api.Features.Auth.Commands;

public record RegisterWithPhoneCommand(string PhoneNumber, string Password) : ICommand<Success>;
