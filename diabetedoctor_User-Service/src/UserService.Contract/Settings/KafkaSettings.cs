namespace UserService.Contract.Settings;

public class KafkaSettings
{
    public const string SectionName = "KafkaSettings";
    public string BootstrapServer { get; init; } = null!;
    public string SaslUsername { get; init; } = null!;

    public string SaslPassword { get; init; } = null!;
    
    public string UserTopic { get; init; } = null!;
    public string UserConnectionName { get; init; } = null!;
    public string UserTopicConsumerGroup { get; init; } = null!;
    
    public string PatientTopic { get; init; } = null!;
    public string PatientConnectionName { get; init; } = null!;
    public string PatientTopicConsumerGroup { get; init; } = null!;
    
    public string HospitalTopic { get; init; } = null!;
    public string HospitalConnectionName { get; init; } = null!;
    public string HospitalTopicConsumerGroup { get; init; } = null!;
    
    public string ConsultationTopic { get; init; } = null!;
    public string ConsultationConnectionName { get; init; } = null!;
    public string ConsultationTopicConsumerGroup { get; init; } = null!;
    public string DeadTopic { get; init; } = null!;
    
    public string RetryTopic { get; init; } = null!;
}