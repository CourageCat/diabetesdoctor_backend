using ConsultationService.Contract.Services.ConsultationTemplate;

namespace ConsultationService.Contract.DTOs.ConsultationTemplateDtos.Requests;

public record CreateConsultationTemplateRequest(IEnumerable<TimeTemplate> TimeTemplates);
