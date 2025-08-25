using MediaService.Contract.Services.Post;
using MongoDB.Bson;

namespace MediaService.Contract.Services.Category;

public record GetAllCategoriesQuery(string UserId)
  : IQuery<Success<IEnumerable<CategoryResponse>>>;

public record GetAllCategoriesByAdminQuery(int PageIndex, int PageSize, Filter.CategoryFilter FilterParams, string SortType, bool IsSortAsc) : IQuery<Success<PagedResult<CategoryResponse>>>;


public record GetCategoryByIdQuery(ObjectId Id) : IQuery<Success<CategoryResponse>>;
public record GetTopPostCategoriesQuery(string UserId, int NumberOfCategories) : IQuery<Success<IEnumerable<CategoryResponse>>>;
