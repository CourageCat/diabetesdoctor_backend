using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Models;
using MongoDB.Bson;
using System.Data.Entity;
using MediaService.Domain.Abstractions;

namespace MediaService.Persistence.Repositories;

public class PostRepository(IMongoDbContext context) : RepositoryBase<Post>(context), IPostRepository
{
    public async Task<UpdateResult> UpdateLikePost(IClientSessionHandle session, bool isLiked, ObjectId postId, CancellationToken cancellationToken)
    {
        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
        var update = isLiked switch
        {
            true => Builders<Post>.Update.Inc(p => p.Like, 1),
            _ => Builders<Post>.Update.Inc(p => p.Like, -1),
        };
        var option = new UpdateOptions<Post>
        {
            IsUpsert = false
        };
        return await DbSet.UpdateOneAsync(session, filter, update, option, cancellationToken);
    }

    public async Task<UpdateResult> UpdateViewPost(IClientSessionHandle session, ObjectId postId, CancellationToken cancellationToken)
    {
        var filter = Builders<Post>.Filter.Eq(p => p.Id, postId);
        var update = Builders<Post>.Update.Inc(p => p.View, 1);
        var option = new UpdateOptions<Post>
        {
            IsUpsert = false
        };
        return await DbSet.UpdateOneAsync(session, filter, update, option, cancellationToken);
    }
}
