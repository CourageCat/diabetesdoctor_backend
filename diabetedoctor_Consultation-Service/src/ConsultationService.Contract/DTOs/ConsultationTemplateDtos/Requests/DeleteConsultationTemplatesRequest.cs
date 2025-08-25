namespace ConsultationService.Contract.DTOs.ConsultationTemplateDtos.Requests;

public record DeleteConsultationTemplatesRequest(HashSet<string> TemplateIds);