namespace ConsultationService.Contract.Services.ConsultationTemplate;

public record TimeRange
{
    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
}