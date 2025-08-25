namespace NotificationService.Contract.Settings;

public class AblySettings
{
    public const string SectionName = "AblySettings";
    public string ApiKey { get; set; } = default!;
    public string Channel { get; set; } = default!;
    public string MessageType { get; set; } = default!;
}
