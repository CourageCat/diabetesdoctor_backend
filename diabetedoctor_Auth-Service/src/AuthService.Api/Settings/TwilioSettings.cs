namespace AuthService.Api.Settings;

public class TwilioSettings
{
    public const string SectionName = "TwilioSettings";
    public string AccountSid { get; init; } = null!;
    public string AuthToken  { get; init; } = null!;
    public string ServiceSid { get; init; } = null!;
    public string ToPhoneNumber { get; init; } = null!;
}