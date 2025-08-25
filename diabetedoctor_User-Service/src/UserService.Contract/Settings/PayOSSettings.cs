namespace UserService.Contract.Settings;

public class PayOSSettings
{
    public const string SectionName = "PayOSSettings";
    public string ClientId { get; set; }
    public string ApiKey { get; set; }
    public string ChecksumKey { get; set; }
    public string SuccessUrl { get; set; }
    public string ErrorUrl { get; set; }
}
