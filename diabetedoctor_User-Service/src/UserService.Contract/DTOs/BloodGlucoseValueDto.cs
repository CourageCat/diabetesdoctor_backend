namespace UserService.Contract.DTOs;

public sealed record BloodGlucoseValueDto
    (string Type,
    double Value,
    string Unit,
    BloodGlucoseLevelEnum Level,
    BloodGlucoseMeasureTimeEnum MeasureTime)
    : HealthRecordValueDto(Type);
