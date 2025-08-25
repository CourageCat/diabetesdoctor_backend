namespace ConsultationService.Contract.Services.ConsultationTemplate.Filters;

public record GetDoctorConsultationTemplatesFilter
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    
    public DateTime? Month { get; init; }
};