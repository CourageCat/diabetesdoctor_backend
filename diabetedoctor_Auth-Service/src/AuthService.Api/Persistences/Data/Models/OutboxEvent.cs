namespace AuthService.Api.Persistences.Data.Models;

public class OutboxEvent : DomainEntity<Guid>
{
    public string Topic { get; private init; } = null!;
    public string EventType { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? VisibleAt { get; private set; }
    public string ErrorMessage { get; private set; } = null!;
    public int RetryCount { get; private set; }
    
    public static OutboxEvent Create(Guid id, string topic, string eventTypeName, string message, int retryCount,
        int delayMinutes)
    {
        return new OutboxEvent
        {
            Id = id,
            Topic = topic,
            EventType = eventTypeName,
            Message = message,
            ProcessedAt = null,
            VisibleAt = DateTime.UtcNow.AddMinutes(delayMinutes),
            ErrorMessage = string.Empty,
            RetryCount = retryCount,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow,
            IsDeleted = false
        };
    }

    public void IncreaseRetryCount()
    {
        RetryCount++;
    }

    public void UpdateProcessedAt()
    {
        ProcessedAt = DateTime.UtcNow;
    }

    public void UpdateErrorMessage(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
}