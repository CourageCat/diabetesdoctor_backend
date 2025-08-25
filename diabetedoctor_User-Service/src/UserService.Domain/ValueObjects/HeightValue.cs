using System.Text.Json.Serialization;

namespace UserService.Domain.ValueObjects;

/// <summary>
/// Giá trị cân nặng
/// </summary>
public sealed class HeightValue : HealthRecordValue
{
    public string Type => "height";
    public double Value { get; } // Giá trị
    public string Unit { get; } // Đơn vị
    
    [JsonConstructor]
    public HeightValue(double value, string unit)
    {
        Value = value;
        Unit = unit;
    }

    public static HeightValue Of(double value, string unit = "cm")
    {
        if (value < 0)
            throw new ArgumentException("Chỉ số chiều cao không được âm.");
        
        return new HeightValue(value, unit);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}