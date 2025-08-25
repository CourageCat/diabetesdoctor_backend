using ConsultationService.Domain.Abstractions;
using ConsultationService.Domain.Abstractions.Repositories;
using ConsultationService.Domain.Models;

namespace ConsultationService.Persistence.Repositories;

public class ConsultationTemplateRepository(IMongoDbContext context)
    : RepositoryBase<ConsultationTemplate>(context), IConsultationTemplateRepository
{
    
};