using MongoDB.Driver;
using NotificationService.Domain.Abstractions;
using NotificationService.Domain.Abstractions.Repositories;
using NotificationService.Domain.Models;
using NotificationService.Domain.ValueObjects;

namespace NotificationService.Persistence.Repositories;

public class UserRepository(IMongoDbContext context) : RepositoryBase<User>(context), IUserRepository
{
    public async Task<List<UserId>> GetUserIdsAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<User>.Filter.Empty;
        var projection = Builders<User>.Projection.Expression(n => n.UserId);
        
        return await DbSet
            .Find(filter)
            .Project(projection)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<string>> GetDeviceIdsAsync(List<UserId>? userIds = default, CancellationToken cancellationToken = default)
    {
        var filters = new List<FilterDefinition<User>>
        {
            Builders<User>.Filter.Ne(u => u.FcmToken, null),
            Builders<User>.Filter.Ne(u => u.FcmToken, "")
        };
        
        if (userIds is not null)
        {
            filters.Add(Builders<User>.Filter.In(n => n.UserId, userIds));
        }
        
        var andFilter = Builders<User>.Filter.And(filters);
        var projection = Builders<User>.Projection.Expression(n => n.FcmToken);

        return (await DbSet
            .Find(andFilter)
            .Project(projection)
            .ToListAsync(cancellationToken: cancellationToken))!;
    }
}