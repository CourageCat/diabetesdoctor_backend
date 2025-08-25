using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.FavouriteCategory.Responses;
using MediaService.Contract.Services.Post;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Models;
using MediaService.Domain.ValueObjects;
using MediaService.Persistence;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;
using MediaService.Application.Helper;
using MediaService.Domain.Enums;
using static MediaService.Contract.Services.Post.Filter;
namespace MediaService.Application.UseCase.V1.Queries.Post;
public sealed class GetAllPostsQueryBySystemHandler(IMongoDbContext context, INormalizeService normalizeService) : IQueryHandler<GetAllPostsBySystemQuery, Success<PagedResult<PostResponse>>>
{
    public async Task<Result<Success<PagedResult<PostResponse>>>> Handle(GetAllPostsBySystemQuery request, CancellationToken cancellationToken)
    {
        var filter = request.FilterParams;
        var filterExpression = GetExpressionFilter(filter);

        var lookupPipeline = GetLookupPipeline();

        var totalCount = await CountPosts(filterExpression, filter, lookupPipeline, cancellationToken);
        if (totalCount == 0)
        {
            // Return empty List<PostResponse>
            return Result.Success(new Success<PagedResult<PostResponse>>(PostMessage.GetAllPostsSuccessfully.GetMessage().Code, PostMessage.GetAllPostsSuccessfully.GetMessage().Message, new PagedResult<PostResponse>(new List<PostResponse>(), request.PageIndex, request.PageSize, 0, 0)));
        }
        var totalPages = Math.Ceiling(totalCount / (double)request.PageSize);
        var postsFound = await GetPostsAsync(filterExpression, lookupPipeline, filter.CategoryIds, request.SortType, request.IsSortAsc, request.PageIndex, request.PageSize, cancellationToken);
        var result = PagedResult<PostResponse>.Create(postsFound.ToList(), request.PageIndex, request.PageSize, totalCount, totalPages);
        return Result.Success(new Success<PagedResult<PostResponse>>(
            PostMessage.GetAllPostsSuccessfully.GetMessage().Code,
            PostMessage.GetAllPostsSuccessfully.GetMessage().Message, result));
    }

    private FilterDefinition<Domain.Models.Post> GetExpressionFilter(PostFilter filter)
    {
        var builder = Builders<Domain.Models.Post>.Filter;
        var searches = new List<FilterDefinition<Domain.Models.Post>>();
        var filters = new List<FilterDefinition<Domain.Models.Post>>() { builder.Eq(x => x.IsDeleted, false) };
        // Search
        if (!string.IsNullOrEmpty(filter.SearchContent))
        {
            searches.Add(builder.Regex(x => x.TitleNormalize, new BsonRegularExpression(Normalize.GetNormalizeString(filter.SearchContent), "i")));
            searches.Add(builder.Regex(x => x.Content, new BsonRegularExpression(filter.SearchContent, "i")));
        }

        // ModeratorId filter
        if (!string.IsNullOrEmpty(filter.ModeratorId))
        {
            filters.Add(builder.Eq(x => x.ModeratorId, UserId.Of(filter.ModeratorId)));
        }
        // DoctorId filter
        if (!string.IsNullOrEmpty(filter.DoctorId))
        {
            filters.Add(builder.Eq(x => x.DoctorId, UserId.Of(filter.DoctorId)));
        }
        // Status filter
        if (filter.Status is not null)
        {
            var statusFilter = filter.Status switch
            {
                Status.Approved => PostStatus.Approved,
                Status.Rejected => PostStatus.Rejected,
                Status.Pending => PostStatus.Pending,
                _ => PostStatus.Drafted
            };
            filters.Add(builder.Eq(x => x.Status, statusFilter));
        }
        
        var builderSearches = searches.Count != 0 ? builder.Or(searches) : builder.Empty;
        var builderFilters = builder.And(filters);
        return builder.And(builderSearches, builderFilters);
    }

    private async Task<List<PostResponse>> GetPostsAsync(FilterDefinition<Domain.Models.Post> filterExpression, PipelineDefinition<PostCategory, Domain.Models.Category>? lookupPipeline, ObjectId[]? categoryIds, string sortType, bool isSortAsc, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var postProjection = new BsonDocument()
        {
            { "_id", 1 },
            { "title", 1 },
            { "thumbnail", "$thumbnail.public_url" },
            { "created_date", 1 },
            { "modified_date", 1},
            { "view", 1 },
            { "like", 1 },
            { "categories", 1 },
            {
                "moderator", new BsonDocument
                {
                    { "_id", "$moderator.user_id._id" },
                    { "full_name", "$moderator.full_name" },
                    { "public_url", "$moderator.avatar.public_url" },
                }
            },
            {
                "doctor", new BsonDocument
                {
                    { "_id", "$doctor.user_id._id" },
                    { "full_name", "$doctor.full_name" },
                    { "public_url", "$doctor.avatar.public_url" },
                }
            },
            { "status", 1 },
        };
        // Sorting stage
        var sortDefinition = isSortAsc ? Builders<Domain.Models.Post>.Sort.Ascending(new StringFieldDefinition<Domain.Models.Post>(sortType)).Ascending(post => post.Id) : Builders<Domain.Models.Post>.Sort.Descending(new StringFieldDefinition<Domain.Models.Post>(sortType)).Descending(post => post.Id);
        
        // Paging values
        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var skip = (pageIndex - 1) * pageSize;
        
        var documents = await context.Posts.Aggregate()
            //Join
            .Lookup<Domain.Models.PostCategory, Domain.Models.Category, IEnumerable<Domain.Models.Category>, PostResponse>(
                foreignCollection: context.PostCategories,
                let: new BsonDocument("postId", "$_id"),
                lookupPipeline: lookupPipeline,
                @as: "categories"
            )
            .Lookup(
                foreignCollectionName: nameof(Domain.Models.User),
                localField: "moderator_id",
                foreignField: "user_id",
                @as: "moderator"
            )
            .Unwind("moderator", new AggregateUnwindOptions<BsonDocument>
            {
                PreserveNullAndEmptyArrays = true
            })
            .Lookup(
                foreignCollectionName: nameof(Domain.Models.User),
                localField: "doctor_id",
                foreignField: "user_id",
                @as: "doctor"
            )
            .Unwind("doctor", new AggregateUnwindOptions<BsonDocument>
            {
                PreserveNullAndEmptyArrays = true
            }) 
            // Filter by CategoryId
            .AppendStage<BsonDocument>(categoryIds != null && categoryIds.Length != 0
            ? new BsonDocument("$match", new BsonDocument
            {
                { "categories._id", new BsonDocument("$in", new BsonArray(categoryIds)) }
            })
            : new BsonDocument("$match", new BsonDocument()))
            .As<Domain.Models.Post>()
            .Sort(sortDefinition)
            // Filter
            .Match(filterExpression)
            .Skip(skip)
            .Limit(pageSize)
            
            // Select fields
            .Project(postProjection)
            .As<PostResponse>()
            .ToListAsync(cancellationToken);
        return documents;
    }

    private PipelineDefinition<PostCategory, Domain.Models.Category>? GetLookupPipeline()
    {
        var categoryProjection = new BsonDocument()
        {
            { "_id", "$category._id" },
            { "name", "$category.name" },
            { "image_url", "$category.image.public_url" }
        };
        return new EmptyPipelineDefinition<Domain.Models.PostCategory>()
            .Match(new BsonDocument
                { { "$expr", new BsonDocument("$eq", new BsonArray { "$post_id", "$$postId" }) } })
            .Sort(Builders<Domain.Models.PostCategory>.Sort.Descending(postCategory => postCategory.Id))
            .Lookup<Domain.Models.PostCategory, Domain.Models.PostCategory, Domain.Models.Category, PostCategoryDto>(
                foreignCollection: context.Categories,
                localField: postCategory => postCategory.CategoryId,
                foreignField: category => category.Id,
                @as: postCategoryDto => postCategoryDto.Category)
            .Unwind(postCategory => postCategory.Category, new AggregateUnwindOptions<BsonDocument>
            {
                PreserveNullAndEmptyArrays = true
            })
            .Project(categoryProjection)
            .As<Domain.Models.PostCategory, BsonDocument, Domain.Models.Category>();
    }

    private async Task<int> CountPosts(FilterDefinition<Domain.Models.Post> filterExpression, PostFilter filter, 
        PipelineDefinition<PostCategory, Domain.Models.Category>? lookupPipeline, CancellationToken cancellationToken)
    {
        var countResult = await context.Posts.Aggregate()
            .Match(filterExpression)
            .Lookup<Domain.Models.PostCategory, Domain.Models.Category, IEnumerable<Domain.Models.Category>, PostResponse>(
                foreignCollection: context.PostCategories,
                let: new BsonDocument("postId", "$_id"),
                lookupPipeline: lookupPipeline,
                @as: "categories"
            )
            .AppendStage<BsonDocument>(filter.CategoryIds != null && filter.CategoryIds.Length != 0
                ? new BsonDocument("$match", new BsonDocument
                {
                    { "categories._id", new BsonDocument("$in", new BsonArray(filter.CategoryIds)) }
                })
                : new BsonDocument("$match", new BsonDocument()))
            .AppendStage<BsonDocument>(new BsonDocument("$count", "total"))
            .FirstOrDefaultAsync(cancellationToken);
        return countResult?["total"].AsInt32 ?? 0; 
    }
}
