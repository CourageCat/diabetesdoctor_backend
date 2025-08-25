namespace ConsultationService.Contract.Services.ConsultationTemplate;

public record TimeTemplate
{
    public DateOnly Date { get; init; }
    
    public List<TimeRange> Times { get; init; } = [];
}