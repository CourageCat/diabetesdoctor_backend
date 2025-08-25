using ConsultationService.Contract.Enums;

namespace ConsultationService.Contract.Services.Consultation.Filters;

public record GetConsultationHistoriesFilter
{
    public ConsultationStatusEnum? Status { get; init; }
};