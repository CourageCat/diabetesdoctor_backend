namespace UserService.Contract.Settings;

public class GeminiSettings
{
    public const string SectionName = "GeminiSettings";
    public string ApiKey { get; set; }
    public string Url { get; set; }
}