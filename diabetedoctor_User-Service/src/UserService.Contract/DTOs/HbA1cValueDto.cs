namespace UserService.Contract.DTOs;

public sealed record HbA1cValueDto
    (string Type,
    double Value,
    string Unit)
    : HealthRecordValueDto(Type);
