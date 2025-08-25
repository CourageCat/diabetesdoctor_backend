namespace ConsultationService.Contract.DTOs.ConsultationTemplateDtos;

public record TimeRangeDto
{
    public TimeSpan StartTime { get; init; }
    public TimeSpan EndTime { get; init; }
};