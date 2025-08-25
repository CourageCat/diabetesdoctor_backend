using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Models;

namespace MediaService.Persistence.Repositories;
public class LikeRepository(IMongoDbContext context) : RepositoryBase<Like>(context), ILikeRepository
{
}
