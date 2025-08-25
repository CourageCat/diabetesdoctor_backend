using ConsultationService.Contract.Enums;
using ConsultationService.Contract.Services.ConsultationTemplate;
using MongoDB.Bson;

namespace ConsultationService.Contract.DTOs.ConsultationTemplateDtos.Requests;

public record UpdateConsultationTemplatesRequest(ConsultationTemplateStatusEnum? Status, List<UpsertTimeTemplate> UpsertTimeTemplates, IEnumerable<string> TemplateIdsToDelete);