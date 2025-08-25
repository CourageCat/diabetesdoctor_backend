namespace UserService.Contract.DTOs;

public sealed record BloodPressureValueDto(string Type, double Systolic, double Diastolic, string Unit)
    : HealthRecordValueDto(Type);