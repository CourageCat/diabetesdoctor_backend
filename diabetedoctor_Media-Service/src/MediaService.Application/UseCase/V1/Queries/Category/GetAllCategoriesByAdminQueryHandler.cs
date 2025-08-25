using MediaService.Contract.Services.Category;

using MediaService.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MediaService.Application.UseCase.V1.Queries.Category;

public sealed class GetAllCategoriesByAdminQueryHandler(IMongoDbContext context) : IQueryHandler<GetAllCategoriesByAdminQuery, Success<PagedResult<CategoryResponse>>>
{
    public async Task<Result<Success<PagedResult<CategoryResponse>>>> Handle(GetAllCategoriesByAdminQuery request, CancellationToken cancellationToken)
    {
        var filter = request.FilterParams;
        var filterExpression = GetExpressionFilter(filter);
        var countResult = await context.Categories.Aggregate()
            .Match(filterExpression)
            .AppendStage<BsonDocument>(new BsonDocument("$count", "total"))
            .FirstOrDefaultAsync(cancellationToken);
        var totalCount = countResult?["total"].AsInt32 ?? 0;
        if (totalCount == 0)
        {
            // Return empty List<PostResponse>
            return Result.Success(new Success<PagedResult<CategoryResponse>>(CategoryMessage.GetAllCategoriesSuccessfully.GetMessage().Code, CategoryMessage.GetAllCategoriesSuccessfully.GetMessage().Message, new PagedResult<CategoryResponse>(new List<CategoryResponse>(), request.PageIndex, request.PageSize, 0, 0)));
        }
        var totalPages = Math.Ceiling(totalCount / (double)request.PageSize);
        var categoriesFound = await GetPostsAsync(filterExpression, request.SortType, request.IsSortAsc, request.PageIndex, request.PageSize, cancellationToken);
        var result = PagedResult<CategoryResponse>.Create(categoriesFound.ToList(), request.PageIndex, request.PageSize, totalCount, totalPages);
        return Result.Success(new Success<PagedResult<CategoryResponse>>(
            CategoryMessage.GetAllCategoriesSuccessfully.GetMessage().Code,
            CategoryMessage.GetAllCategoriesSuccessfully.GetMessage().Message, result));
    }
    
    private FilterDefinition<Domain.Models.Category> GetExpressionFilter(Filter.CategoryFilter filter)
    {
        var builder = Builders<Domain.Models.Category>.Filter;
        var searches = new List<FilterDefinition<Domain.Models.Category>>();
        // Search
        if (!string.IsNullOrEmpty(filter.SearchContent))
        {
            searches.Add(builder.Regex(x => x.Name, new BsonRegularExpression(Normalize.GetNormalizeString(filter.SearchContent), "i")));
            searches.Add(builder.Regex(x => x.Description, new BsonRegularExpression(filter.SearchContent, "i")));
        }
        
        var builderSearches = searches.Count != 0 ? builder.Or(searches) : builder.Empty;

        return builder.And(builderSearches);
    }
    
        private async Task<List<CategoryResponse>> GetPostsAsync(FilterDefinition<Domain.Models.Category> filterExpression, string sortType, bool isSortAsc, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {

        var categoryProjection = new BsonDocument()
        {
            { "_id", 1 },
            { "name", 1 },
            { "image_url", "$image.public_url" },
            { "created_date", 1 },
        };
        // Sorting stage
        var sortDefinition = isSortAsc ? Builders<Domain.Models.Category>.Sort.Ascending(new StringFieldDefinition<Domain.Models.Category>(sortType)).Ascending(category => category.Id) : Builders<Domain.Models.Category>.Sort.Descending(new StringFieldDefinition<Domain.Models.Category>(sortType)).Descending(category => category.Id);
        
        // Paging values
        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var skip = (pageIndex - 1) * pageSize;
        
        
        var documents = await context.Categories.Aggregate()
            .Match(filterExpression)
            .Sort(sortDefinition)
            .Skip(skip)
            .Limit(pageSize)
            .Project(categoryProjection)
            .As<CategoryResponse>()
            .ToListAsync(cancellationToken);
        return documents;
    }

}
