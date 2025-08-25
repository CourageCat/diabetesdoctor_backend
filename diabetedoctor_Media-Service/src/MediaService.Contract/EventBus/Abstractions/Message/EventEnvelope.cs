namespace MediaService.Contract.EventBus.Abstractions.Message;

public class EventEnvelope
{
    public string EventTypeName { get; set; } = default!;
    public string Message { get; set; } = default!;
    public int RetryCount { get; set; }

    public EventEnvelope() {}
    
    public EventEnvelope(Type type, string eventMessage, int retryCount): this(type.Name!, eventMessage, retryCount) {}

    private EventEnvelope(string eventTypeName, string eventMessage, int  retryCount)
    {
        EventTypeName = eventTypeName ?? throw new ArgumentNullException(nameof(eventTypeName));
        Message = eventMessage ?? throw new ArgumentNullException(nameof(eventMessage));
        RetryCount = retryCount;
    }
}