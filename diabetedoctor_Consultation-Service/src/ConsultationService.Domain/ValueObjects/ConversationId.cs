using ConsultationService.Domain.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Domain.ValueObjects;

public sealed class ConversationId : ValueObject
{
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; private init; } = null!;
    
    private ConversationId(){}

    private ConversationId(string id)
    {
        Id = id;
    }

    public static ConversationId Of(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Id bắt buộc phải có");
        }
        return new ConversationId(id);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
    }
    
    public override string ToString() => Id;
}