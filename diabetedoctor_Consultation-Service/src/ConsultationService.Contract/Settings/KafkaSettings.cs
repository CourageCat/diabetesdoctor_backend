namespace ConsultationService.Contract.Settings;

public class KafkaSettings
{
    public const string SectionName = "KafkaSettings";
    
    public string BootstrapServer { get; init; } = null!;
    public string SaslUsername { get; init; } = null!;
    public string SaslPassword { get; init; } = null!;
    
    public string SourceName { get; init; } = null!;
    
    // -- topic --
    
    // dead
    public string DeadTopic { get; init; } = null!;
    
    // retry
    public string RetryTopic { get; init; } = null!;
    public string RetryConnectionName { get; init; } = null!;
    public string RetryTopicConsumerGroup { get; init; } = null!;
    
    // consultation
    public string ConsultationTopic { get; init; } = null!;
    public string ConsultationConnectionName { get; init; } = null!;
    public string ConsultationTopicConsumerGroup { get; init; } = null!;
    
    // user
    public string UserTopic { get; init; } = null!;
    public string UserConnectionName { get; init; } = null!;
    public string UserTopicConsumerGroup { get; init; } = null!;
    
    public string ConversationTopic { get; init; } = null!;
    public string ConversationConnectionName { get; init; } = null!;
    public string ConversationTopicConsumerGroup { get; init; } = null!;
}