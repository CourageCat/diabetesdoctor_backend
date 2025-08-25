using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.Abstractions.Shared;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Commands;

public class DeleteConsultationTemplatesCommand : ICommand<Response>
{
    public string StaffId { get; init; } = null!;
    public string DoctorId { get; init; } = null!;
    public IEnumerable<string> TemplateIds { get; init; } = [];
}