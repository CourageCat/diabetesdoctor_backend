using System.Text.Json.Serialization;

namespace UserService.Domain.ValueObjects;

/// <summary>
/// Giá trị cân nặng
/// </summary>
public sealed class MaxConsultationValue : PackageFeatureValue
{
    public string Type => "max_consultation";
    public int Value { get; } // Giá trị
    
    [JsonConstructor]
    public MaxConsultationValue(int value)
    {
        Value = value;
    }

    public static MaxConsultationValue Of(int value)
    {
        if (value < 0)
            throw new ArgumentException("Số lượt tư vấn không được âm.");
        
        return new MaxConsultationValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}