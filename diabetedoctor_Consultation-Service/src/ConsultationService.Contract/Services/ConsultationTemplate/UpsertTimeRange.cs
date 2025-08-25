namespace ConsultationService.Contract.Services.ConsultationTemplate;

public record UpsertTimeRange
{
    public TimeOnly? Start { get; init; }
    public TimeOnly? End { get; init; }
};