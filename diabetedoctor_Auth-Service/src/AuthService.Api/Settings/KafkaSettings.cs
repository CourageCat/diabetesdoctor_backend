namespace AuthService.Api.Settings;

public class KafkaSettings
{
    public const string SectionName = "KafkaSettings";
    public string BootstrapServer { get; init; } = default!;
    public string SaslUsername { get; init; } = default!;
    public string SaslPassword { get; init; } = default!;
    
    public string UserTopic { get; init; } = null!;
    public string UserConnectionName { get; init; } = null!;
    public string UserTopicConsumerGroup { get; init; } = null!;
    
    public string DeadTopic { get; init; } = null!;
    
    public string RetryTopic { get; init; } = null!;
}