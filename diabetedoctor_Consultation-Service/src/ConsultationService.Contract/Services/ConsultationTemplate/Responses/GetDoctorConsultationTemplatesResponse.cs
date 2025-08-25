using ConsultationService.Contract.DTOs.ConsultationTemplateDtos.Responses;

namespace ConsultationService.Contract.Services.ConsultationTemplate.Responses;

public record GetDoctorConsultationTemplatesResponse
{
    public DateTime Date { get; init; }
    public IEnumerable<ConsultationTemplateResponseDto> ConsultationTemplates { get; init; } = [];
}