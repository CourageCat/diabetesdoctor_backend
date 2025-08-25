namespace ChatService.Domain.ValueObjects;

public sealed class ConsultationId : ValueObject
{
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; private init; } = null!;
    
    private ConsultationId() { }
    
    private ConsultationId(string id)
    {
        Id = id;    
    }
    
    public static ConsultationId Of(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Id bắt buộc phải có");
        }
        return new ConsultationId(id);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
    }
    
    public override string ToString() => Id;
}