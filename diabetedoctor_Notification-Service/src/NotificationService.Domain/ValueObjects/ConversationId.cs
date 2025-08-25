using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Domain.Abstractions;

namespace NotificationService.Domain.ValueObjects;

public sealed class ConversationId : ValueObject
{
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private init; } = null!;

    public static ConversationId Of(string id)
    {
        return new ConversationId {Id = id};
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
    }

    public override string ToString() => Id;
}