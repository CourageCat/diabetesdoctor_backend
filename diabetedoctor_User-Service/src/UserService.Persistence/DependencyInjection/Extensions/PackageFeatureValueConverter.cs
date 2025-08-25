using System.Text.Json;
using System.Text.Json.Serialization;
using UserService.Domain.ValueObjects;

namespace UserService.Persistence.DependencyInjection.Extensions;

public class PackageFeatureValueConverter : JsonConverter<PackageFeatureValue>
{
    public override PackageFeatureValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var type = doc.RootElement.GetProperty("Type").GetString();

        return type switch
        {
            "max_consultation" => JsonSerializer.Deserialize<MaxConsultationValue>(doc.RootElement.GetRawText(), options),
            "additional_notes" => JsonSerializer.Deserialize<AdditionalNotesValue>(doc.RootElement.GetRawText(), options),
            _ => throw new JsonException($"Unknown type {type}")
        };
    }

    public override void Write(Utf8JsonWriter writer, PackageFeatureValue value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case MaxConsultationValue maxConsultation:
                JsonSerializer.Serialize(writer, maxConsultation, options);
                break;
            case AdditionalNotesValue additionalNotes:
                JsonSerializer.Serialize(writer, additionalNotes, options);
                break;
            default:
                throw new JsonException($"Unknown subtype {value.GetType()}");
        }
    }
}
