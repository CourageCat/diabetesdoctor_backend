using AuthService.Api.Features.Auth.Responses;

namespace AuthService.Api.Features.Auth.Commands;

public record VerifyOtpRegisterCommand(string PhoneNumber, string Otp) : ICommand<Success<LoginResponse>>;