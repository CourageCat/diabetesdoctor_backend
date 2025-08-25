namespace AuthService.Api.Features.Auth.Commands;

public record ResendOtpRegisterCommand(string PhoneNumber) : ICommand<Success>;
