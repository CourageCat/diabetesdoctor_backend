using System.Text.Json.Serialization;

namespace UserService.Contract.DTOs;

public record MeasurementNoteResponseDto(
    [property: JsonPropertyName("patientId")] string PatientId,
    [property: JsonPropertyName("recordTime")] string RecordTime,
    [property: JsonPropertyName("feedback")] string FeedBack
);