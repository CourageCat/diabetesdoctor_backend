using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Models;
using MongoDB.Bson;
using System.Data.Entity;
using MediaService.Domain.Abstractions;

namespace MediaService.Persistence.Repositories;

public class PostCategoryRepository(IMongoDbContext context) : RepositoryBase<PostCategory>(context), IPostCategoryRepository
{
}
