using System.Text.Json.Serialization;

namespace UserService.Domain.ValueObjects;

/// <summary>
/// Giá trị cân nặng
/// </summary>
public sealed class AdditionalNotesValue : PackageFeatureValue
{
    public string Type => "additional_notes";
    public string Value { get; } // Giá trị
    
    [JsonConstructor]
    public AdditionalNotesValue(string value)
    {
        Value = value;
    }

    public static AdditionalNotesValue Of(string value)
    {
        
        return new AdditionalNotesValue(value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}