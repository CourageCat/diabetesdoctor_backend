namespace UserService.Contract.DTOs;

public sealed record NoteValueDto(string Type, string Value, string? Note)
    : HealthRecordValueDto(Type);
