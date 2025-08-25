namespace UserService.Contract.DTOs;

public sealed record HeightValueDto(string Type, double Value, string Unit)
    : HealthRecordValueDto(Type);
