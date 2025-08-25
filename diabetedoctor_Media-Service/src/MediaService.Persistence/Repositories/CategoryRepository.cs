using MediaService.Domain.Abstractions;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Models;

namespace MediaService.Persistence.Repositories;

public class CategoryRepository(IMongoDbContext context) : RepositoryBase<Category>(context), ICategoryRepository
{
}
