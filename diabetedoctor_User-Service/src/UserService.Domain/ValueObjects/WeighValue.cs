using System.Text.Json.Serialization;

namespace UserService.Domain.ValueObjects;

/// <summary>
/// Giá trị cân nặng
/// </summary>
public sealed class WeightValue : HealthRecordValue
{
    public string Type => "weight";
    public double Value { get; } // Giá trị
    public string Unit { get; } // Đơn vị

    [JsonConstructor]
    public WeightValue(double value, string unit)
    {
        Value = value;
        Unit = unit;
    }

    public static WeightValue Of(double value, string unit = "kg")
    {
        if (value < 0)
            throw new ArgumentException("Chỉ số cân nặng không được âm.");
        
        return new WeightValue(value, unit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}