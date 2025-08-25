using MediaService.Domain.Models;
using MongoDB.Bson;
using System.Linq.Expressions;
namespace MediaService.Domain.Abstractions.Repositories;
public interface IPostCategoryRepository : IRepositoryBase<PostCategory>
{
}
