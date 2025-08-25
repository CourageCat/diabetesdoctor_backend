namespace UserService.Contract.DTOs.HealthRecord;

public record RequestForAiNoteDto
{
    public string MeasurementType { get; init; } = string.Empty;
    public string Value { get; init; } = string.Empty;
    public string Context { get; init; } = string.Empty;
    public string Time { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
}