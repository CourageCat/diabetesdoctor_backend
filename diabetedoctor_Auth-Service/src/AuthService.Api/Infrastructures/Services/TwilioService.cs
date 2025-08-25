using Twilio.Rest.Verify.V2.Service;

namespace AuthService.Api.Infrastructures.Services;

public sealed class TwilioService : ITwilioService
{
    private readonly TwilioSettings _twilioSettings;
    
    public TwilioService(IOptions<TwilioSettings> options)
    {
        _twilioSettings = options.Value;
    }
    public async Task SendOtp(string otp)
    {
        await VerificationResource.CreateAsync(
            pathServiceSid: _twilioSettings.ServiceSid,
            to: _twilioSettings.ToPhoneNumber,
            customCode: otp,
            channel: VerificationResource.ChannelEnum.Sms.ToString());
    }
}