namespace UserService.Contract.DTOs;

public sealed record WeightValueDto(string Type, double Value, string Unit)
    : HealthRecordValueDto(Type);
