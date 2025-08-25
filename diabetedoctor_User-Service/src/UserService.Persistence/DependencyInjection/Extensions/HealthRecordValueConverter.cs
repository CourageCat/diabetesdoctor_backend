using System.Text.Json;
using System.Text.Json.Serialization;
using UserService.Domain.ValueObjects;

namespace UserService.Persistence.DependencyInjection.Extensions;

public class HealthRecordValueConverter : JsonConverter<HealthRecordValue>
{
    public override HealthRecordValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var type = doc.RootElement.GetProperty("Type").GetString();

        return type switch
        {
            "weight" => JsonSerializer.Deserialize<WeightValue>(doc.RootElement.GetRawText(), options),
            "height" => JsonSerializer.Deserialize<HeightValue>(doc.RootElement.GetRawText(), options),
            "blood_glucose" => JsonSerializer.Deserialize<BloodGlucoseValue>(doc.RootElement.GetRawText(), options),
            "blood_pressure" => JsonSerializer.Deserialize<BloodPressureValue>(doc.RootElement.GetRawText(), options),
            "hba1c" => JsonSerializer.Deserialize<HbA1cValue>(doc.RootElement.GetRawText(), options),
            _ => throw new JsonException($"Unknown type {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, HealthRecordValue value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case WeightValue w:
                JsonSerializer.Serialize(writer, w, options);
                break;
            case HeightValue h:
                JsonSerializer.Serialize(writer, h, options);
                break;
            case BloodGlucoseValue bg:
                JsonSerializer.Serialize(writer, bg, options);
                break;
            case BloodPressureValue bp:
                JsonSerializer.Serialize(writer, bp, options);
                break;
            case HbA1cValue hba1c:
                JsonSerializer.Serialize(writer, hba1c, options);
                break;
            default:
                throw new JsonException($"Unknown subtype {value.GetType()}");
        }
    }
}
