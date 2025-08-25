using System.Text.Json.Serialization;

namespace UserService.Contract.DTOs;

public record CarePlanMeasurementResponseDto(
    [property: JsonPropertyName("patientId")] string PatientId,
    [property: JsonPropertyName("aiGeneratedAt")] DateTime AiGeneratedAt,
    [property: JsonPropertyName("schedules")] List<CarePlanMeasurementItemDto> Schedules
);