namespace MediaService.Contract.Settings;

public class AuthSettings
{
    public const string SectionName = "AuthSettings";
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string AccessSecretToken { get; set; } = default!;
    public string RefreshSecretToken { get; set; } = default!;
    public double AccessTokenExpMinute { get; set; } = default!;
    public double RefreshTokenExpMinute { get; set; } = default!;
    public string ForgotPasswordSecretToken { get; set; } = default!;
    public double ForgotPasswordExpMinute { get; set; } = default!;
}

