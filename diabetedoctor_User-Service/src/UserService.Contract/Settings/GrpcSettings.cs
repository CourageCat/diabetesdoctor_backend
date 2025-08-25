namespace UserService.Contract.Settings;

public class GrpcSettings
{
    public const string SectionName = "GrpcSettings";
    public string Url { get; set; } = null!;
}