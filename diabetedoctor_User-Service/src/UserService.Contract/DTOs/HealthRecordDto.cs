namespace UserService.Contract.DTOs;

public record HealthRecordDto(
    Guid? PatientId,
    RecordEnum? RecordType,
    object? HealthRecord = null,
    DateTime? MesurementAt = null,
    string? PersonNote = null,
    string? AssistantNote = null
);