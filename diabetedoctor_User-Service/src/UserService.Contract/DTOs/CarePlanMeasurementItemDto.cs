using System.Text.Json;
using System.Text.Json.Serialization;

namespace UserService.Contract.DTOs;
public record CarePlanMeasurementItemDto
{
    [JsonPropertyName("recordType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RecordEnum RecordType { get; init; }

    [JsonPropertyName("period")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public HealthCarePlanPeriodEnum Period { get; init; }

    [JsonPropertyName("subtype")]
    [JsonConverter(typeof(NullableStringEnumConverter<HealthCarePlanSubTypeEnum>))]
    public HealthCarePlanSubTypeEnum? Subtype { get; init; }

    [JsonPropertyName("reason")]
    public string Reason { get; init; }
}

public class NullableStringEnumConverter<T> : JsonConverter<T?> where T : struct, Enum
{
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString();
        if (string.IsNullOrWhiteSpace(str) || str == "null")
            return null;

        if (Enum.TryParse<T>(str, ignoreCase: true, out var result))
            return result;

        throw new JsonException($"Invalid enum value '{str}' for type {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteStringValue(value.Value.ToString());
        else
            writer.WriteNullValue();
    }
}
