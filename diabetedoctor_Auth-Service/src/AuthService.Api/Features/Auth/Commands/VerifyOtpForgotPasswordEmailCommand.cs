namespace AuthService.Api.Features.Auth.Commands;

public record VerifyOtpForgotPasswordEmailCommand(string Email, string Otp, string Password) : ICommand<Success>;