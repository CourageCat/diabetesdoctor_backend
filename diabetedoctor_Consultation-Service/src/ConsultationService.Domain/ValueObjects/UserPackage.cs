using System.Globalization;
using ConsultationService.Domain.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConsultationService.Domain.ValueObjects;

public sealed class UserPackage : ValueObject
{
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; private init; } = null!;
    
    [BsonElement("price")]
    public double Price { get; private init; }
    
    private UserPackage(){}

    private UserPackage(string id, double price)
    {
        Id = id;
        Price = price;
    }

    public static UserPackage Create(string id, double price)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Id bắt buộc phải có");
        }
        
        return new UserPackage(id, price);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Id;
        yield return Price;
    }

    public override string ToString() => Id;
}