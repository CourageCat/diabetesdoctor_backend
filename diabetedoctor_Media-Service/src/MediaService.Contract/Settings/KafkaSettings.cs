namespace MediaService.Contract.Settings;

public class KafkaSettings
{
    public const string SectionName = "KafkaSettings";
    public string BootstrapServer { get; init; } = null!;
    public string SaslUsername { get; init; } = null!;

    public string SaslPassword { get; init; } = null!;
    
    public string UserTopic { get; init; } = null!;
    public string UserConnectionName { get; init; } = null!;
    public string UserTopicConsumerGroup { get; init; } = null!;
    
    public string PostTopic { get; init; } = null!;
    
    public string DeadTopic { get; init; } = null!;
    
    public string RetryTopic { get; init; } = null!;
}