using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.Abstractions.Shared;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Commands;

public record CreateConsultationTemplatesCommand : ICommand<Response>
{
    public string StaffId { get; init; } = null!;
    public string DoctorId { get; init; } = null!;
    public IEnumerable<TimeTemplate> TimeTemplates { get; init; } = [];
}