namespace AuthService.Api.Infrastructures.Abstractions.Services;

public interface ITwilioService
{
    
    Task SendOtp(string otp);
}
