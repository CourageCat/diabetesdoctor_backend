using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.Abstractions.Shared;

namespace ConsultationService.Contract.Services.Consultation.Commands;

public record BookConsultationCommand : ICommand<Response>
{
    public string TemplateId { get; init; } = null!;
    public string UserId { get; init; } = null!;
}