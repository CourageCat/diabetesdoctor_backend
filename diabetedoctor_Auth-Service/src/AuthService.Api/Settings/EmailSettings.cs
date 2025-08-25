namespace AuthService.Api.Settings;

public class EmailSettings
{
    public const string SectionName = "EmailSettings";
    public string EmailHost { get; set; } = default!;
    public string EmailUsername { get; set; } = default!;
    public string EmailPassword { get; set; } = default!;
}