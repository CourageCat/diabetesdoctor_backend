using ConsultationService.Contract.Helpers;
using ConsultationService.Domain.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Domain.Models;

public class OutboxScheduleEvent : DomainEntity<ObjectId>
{
    [BsonElement("kafka_topic")]
    public string Topic { get; private init; } = null!;
    [BsonElement("event_type")]
    public string EventType { get; private set; } = null!;
    [BsonElement("message")]
    public string Message { get; private set; } = null!;
    [BsonElement("visible_at")]
    public DateTime VisibleAt  { get; private set; }
    [BsonElement("is_created")]
    public bool IsCreated {get; private set;}
    
    public static OutboxScheduleEvent Create(ObjectId id, string topic, string eventTypeName, string message, DateTime visibleAt)
    {
        return new OutboxScheduleEvent
        {
            Id = id,
            Topic = topic,
            EventType = eventTypeName,
            Message = message,
            VisibleAt = visibleAt,
            IsCreated = false,
            CreatedDate = CurrentTimeService.GetCurrentTimeUtc(),
            ModifiedDate = CurrentTimeService.GetCurrentTimeUtc(),
            IsDeleted = false
        };
    }
}