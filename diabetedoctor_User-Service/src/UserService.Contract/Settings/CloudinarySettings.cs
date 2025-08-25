namespace UserService.Contract.Settings;

public class CloudinarySettings
{
    public const string SectionName = "CloudinarySettings";
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
    public string Folder { get; set; }
}
