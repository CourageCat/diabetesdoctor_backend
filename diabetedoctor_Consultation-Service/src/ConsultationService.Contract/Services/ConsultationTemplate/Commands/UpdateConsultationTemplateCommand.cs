using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.Abstractions.Shared;
using ConsultationService.Contract.Enums;
using MongoDB.Bson;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Commands;

public record UpdateConsultationTemplateCommand : ICommand<Response>
{
    public string StaffId { get; init; } = null!;
    public string DoctorId { get; init; } = null!;
    public ConsultationTemplateStatusEnum? Status { get; init; }
    public List<UpsertTimeTemplate> UpsertTimeTemplates { get; init; } = [];
    public IEnumerable<string> TemplateIdsToDelete { get; init; } = [];
}