using MongoDB.Bson;

namespace ConsultationService.Contract.Services.ConsultationTemplate;

public record UpsertTimeTemplate
{
    public string? TimeTemplateId { get; init; }
    public DateOnly? Date { get; init; }
    public UpsertTimeRange TimeRange { get; init; } = null!;
}