using ConsultationService.Domain.Models;
using ConsultationService.Domain.ValueObjects;
using MongoDB.Bson;

namespace ConsultationService.Domain.Abstractions.Repositories;

public interface IUserRepository : IRepositoryBase<User>
{
    Task<BsonDocument?> GetUserWithHospital(UserId userId, CancellationToken cancellationToken);
}