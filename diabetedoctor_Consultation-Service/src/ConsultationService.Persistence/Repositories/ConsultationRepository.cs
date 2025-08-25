using ConsultationService.Domain.Abstractions;
using ConsultationService.Domain.Abstractions.Repositories;
using ConsultationService.Domain.Models;

namespace ConsultationService.Persistence.Repositories;

public class ConsultationRepository(IMongoDbContext context) : RepositoryBase<Consultation>(context), IConsultationRepository;