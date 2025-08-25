using MediaService.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
namespace MediaService.Domain.Abstractions.Repositories;
public interface IPostRepository : IRepositoryBase<Post>
{
    Task<UpdateResult> UpdateLikePost(IClientSessionHandle session, bool isLiked, ObjectId postId, CancellationToken cancellationToken);
    Task<UpdateResult> UpdateViewPost(IClientSessionHandle session, ObjectId postId, CancellationToken cancellationToken);

}
