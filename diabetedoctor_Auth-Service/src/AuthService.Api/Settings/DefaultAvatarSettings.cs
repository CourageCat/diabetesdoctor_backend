namespace AuthService.Api.Settings;

public class DefaultAvatarSettings
{
    public const string SectionName = "DefaultAvatarSettings";
    public string AvatarId { get; set; } = default!;
    public string AvatarUrl { get; set; } = default!;
}

