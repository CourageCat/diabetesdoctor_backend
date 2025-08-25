namespace NotificationService.Contract.Settings;

public class KafkaSettings
{
    public const string SectionName = "KafkaSettings";
    public string BootstrapServer { get; init; } = null!;
    public string SaslUsername { get; init; } = null!;
    public string SaslPassword { get; init; } = null!;
    
    // -- topic --
    
    // dead
    public string DeadTopic { get; init; } = null!;
    
    // retry
    public string RetryTopic { get; init; } = null!;
    public string RetryConnectionName { get; init; } = null!;
    public string RetryTopicConsumerGroup { get; init; } = null!;
    
    // user
    public string UserTopic { get; init; } = null!;
    public string UserConnectionName { get; init; } = null!;
    public string UserTopicConsumerGroup { get; init; } = null!;
    
    // conversation
    public string ConversationTopic { get; init; } = null!;
    public string ConversationConnectionName { get; init; } = null!;
    public string ConversationTopicConsumerGroup { get; init; } = null!;
    
    // chat
    public string ChatTopic { get; init; } = null!;
    public string ChatConnectionName { get; init; } = null!;
    public string ChatTopicConsumerGroup { get; init; } = null!;
    
    // post
    public string PostTopic { get; init; } = null!;
    public string PostConnectionName { get; init; } = null!;
    public string PostTopicConsumerGroup { get; init; } = null!;
    
}