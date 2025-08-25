namespace AuthService.Api.Settings;

public class RedisSettings
{
    public const string SectionName = "RedisSettings";
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = default!;
}
