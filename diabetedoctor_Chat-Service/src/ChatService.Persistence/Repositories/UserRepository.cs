using ChatService.Domain.ValueObjects;
using MongoDB.Bson;

namespace ChatService.Persistence.Repositories;

public class UserRepository(IMongoDbContext context) : RepositoryBase<User>(context), IUserRepository
{
}