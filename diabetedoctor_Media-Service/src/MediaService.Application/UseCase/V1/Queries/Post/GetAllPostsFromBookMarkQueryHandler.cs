using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.Post;
using MediaService.Domain.ValueObjects;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Data.Entity;
using System.Text;
using MediaService.Application.Helper;
using MediaService.Domain.Enums;
using static MediaService.Contract.Services.Post.Filter;

namespace MediaService.Application.UseCase.V1.Queries.Post;
public sealed class GetAllPostsFromBookMarkQueryHandler(IMongoDbContext context) : IQueryHandler<GetAllPostsFromBookMarkQuery, Success<PagedList<PostResponse>>>
{
    public async Task<Result<Success<PagedList<PostResponse>>>> Handle(GetAllPostsFromBookMarkQuery request, CancellationToken cancellationToken)
    {
        var filter = request.FilterParams;
        var cursorValues = new List<string>();
        if(request.Cursor != null)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.Cursor));
            cursorValues.AddRange(decoded.Split('|').ToList());
        }
        var filterExpression = GetExpressionFilter(filter, request.IsSortAsc, request.SortType, cursorValues);
        var postsFromBookMarkFound = await GetAllPostsFromBookMarkAsync(filterExpression, filter.UserId, filter.CategoryIds, request.SortType, (request.SortType.Equals("bookmarkedDate") && cursorValues.Count != 0) ? DateTime.Parse(cursorValues[0]) : null, request.IsSortAsc, request.PageSize, cancellationToken);
        var hasNext = postsFromBookMarkFound.Count > request.PageSize;
        var nextCursor = "";

        if (hasNext)
        {
            postsFromBookMarkFound.RemoveRange(request.PageSize, postsFromBookMarkFound.Count - request.PageSize);
            var lastPostFromBookMark = postsFromBookMarkFound.ToList()[^1];
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{GetSortCursorValue(lastPostFromBookMark, request.SortType)}|{lastPostFromBookMark.Id}"));
        }
        var result = PagedList<PostResponse>.Create(postsFromBookMarkFound.ToList(), request.PageSize, nextCursor, hasNext);
        return Result.Success(new Success<PagedList<PostResponse>>(
            PostMessage.GetAllPostsFromBookMarkSuccessfully.GetMessage().Code,
            PostMessage.GetAllPostsFromBookMarkSuccessfully.GetMessage().Message, result));
    }

    private FilterDefinition<Domain.Models.Post> GetExpressionFilter(PostFilter filter, bool isSortAsc, string sortType, List<string> cursorValues)
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

        // Cursor Filter
        if (cursorValues.Count != 0)
        {
            if (isSortAsc)
            {
                if (sortType.Equals("id"))
                {
                    filters.Add(builder.Gt(x => x.Id, ObjectId.Parse(cursorValues[0])));
                }
                else if (sortType.Equals("title"))
                {
                    filters.Add(builder.Gt(x => x.Title, cursorValues[0]) | ((builder.Eq(x => x.Title, cursorValues[0])) & (builder.Gt(x => x.Id, ObjectId.Parse(cursorValues[1])))));

                }
                else if (sortType.Equals("view"))
                {
                    filters.Add(builder.Gt(x => x.View, int.Parse(cursorValues[0])) | ((builder.Eq(x => x.View, int.Parse(cursorValues[0]))) & (builder.Gt(x => x.Id, ObjectId.Parse(cursorValues[1])))));
                }
                else if (sortType.Equals("like"))
                {
                    filters.Add(builder.Gt(x => x.Like, int.Parse(cursorValues[0])) | ((builder.Eq(x => x.Like, int.Parse(cursorValues[0]))) & (builder.Gt(x => x.Id, ObjectId.Parse(cursorValues[1])))));
                }
                else if (sortType.Equals("createdDate"))
                {
                    filters.Add(builder.Gt(x => x.CreatedDate, DateTime.Parse(cursorValues[0])) | ((builder.Eq(x => x.CreatedDate, DateTime.Parse(cursorValues[0]))) & (builder.Gt(x => x.Id, ObjectId.Parse(cursorValues[1])))));
                }
            }
            else
            {
                if (sortType.Equals("id"))
                {
                    filters.Add(builder.Lt(x => x.Id, ObjectId.Parse(cursorValues[0])));
                }
                else if (sortType.Equals("title"))
                {
                    filters.Add(builder.Lt(x => x.Title, cursorValues[0]) | ((builder.Eq(x => x.Title, cursorValues[0])) & (builder.Lt(x => x.Id, ObjectId.Parse(cursorValues[1])))));

                }
                else if (sortType.Equals("view"))
                {
                    filters.Add(builder.Lt(x => x.View, int.Parse(cursorValues[0])) | ((builder.Eq(x => x.View, int.Parse(cursorValues[0]))) & (builder.Lt(x => x.Id, ObjectId.Parse(cursorValues[1])))));
                }
                else if (sortType.Equals("like"))
                {
                    filters.Add(builder.Lt(x => x.Like, int.Parse(cursorValues[0])) | ((builder.Eq(x => x.Like, int.Parse(cursorValues[0]))) & (builder.Lt(x => x.Id, ObjectId.Parse(cursorValues[1])))));
                }
                else if (sortType.Equals("createdDate"))
                {
                    filters.Add(builder.Lt(x => x.CreatedDate, DateTime.Parse(cursorValues[0])) | ((builder.Eq(x => x.CreatedDate, DateTime.Parse(cursorValues[0]))) & (builder.Lt(x => x.Id, ObjectId.Parse(cursorValues[1])))));
                }
            }
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

    private string GetSortCursorValue(PostResponse post, string sortType)
    {
        var result = sortType switch
        {
            "id" => post.Id.ToString(),
            "title" => post.Title,
            "createdDate" => post.CreatedDate.ToString("o"),
            "view" => post.View.ToString(),
            "like" => post.Like.ToString(),
            "bookmarkedDate" => post.BookmarkedDate != null ? post.BookmarkedDate.Value.ToString("o") : "",
            _ => post.Id.ToString(),
        };
        return result;
    }


    private async Task<List<PostResponse>> GetAllPostsFromBookMarkAsync(FilterDefinition<Domain.Models.Post> filterExpression, string? userId, ObjectId[]? categoryIds, string sortType, DateTime? cursorBookMarkedDate, bool isSortAsc, int pageSize, CancellationToken cancellationToken)
    {
        var postProjection = new BsonDocument()
        {
            { "_id", 1 },
            { "title", 1 },
            { "thumbnail", "$thumbnail.public_url" },
            { "created_date", 1 },
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
            { "status", 1 },
            { "is_bookmarked", 1 },
            { "is_liked", 1 },
            { "word_count", 1 },
            { "bookmarked_date", "$bookmark.created_date" }
        };
        var categoryProjection = new BsonDocument()
        {
            { "_id", "$category._id" },
            { "name", "$category.name" },
            { "image_url", "$category.image.public_url" }
        };
        // Sorting stage
        var sortDefinition = isSortAsc ? Builders<Domain.Models.Post>.Sort.Ascending(new StringFieldDefinition<Domain.Models.Post>(sortType)).Ascending(post => post.Id) : Builders<Domain.Models.Post>.Sort.Descending(new StringFieldDefinition<Domain.Models.Post>(sortType)).Descending(post => post.Id);
        var lookupPipeline = new EmptyPipelineDefinition<Domain.Models.PostCategory>()
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
        var documents = await context.Posts.Aggregate()
            // Only keep posts that have at least one matching bookmark
            .AppendStage<BsonDocument>(new BsonDocument("$lookup", new BsonDocument
            {
                { "from", nameof(Domain.Models.BookMark) },
                { "let", new BsonDocument
                    {
                        { "postId", "$_id" },
                        { "userId", userId ?? ""}
                    }
                },
                { "pipeline", new BsonArray
                    {
                        new BsonDocument("$match", new BsonDocument("$expr",
                            new BsonDocument("$and", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray { "$post_id", "$$postId" }),
                                new BsonDocument("$eq", new BsonArray { "$user_id._id", "$$userId" })
                            })
                        )),
                        new BsonDocument("$limit", 1) // only get 1 bookmark

                    }
                },
                { "as", "bookmark" }
            }))
            .AppendStage<BsonDocument>(new BsonDocument("$unwind", new BsonDocument
            {
                { "path", "$bookmark" },
                { "preserveNullAndEmptyArrays", false } // Remove post if there's no bookmark
            }))
            .AppendStage<BsonDocument>(new BsonDocument("$match", new BsonDocument
            {
                { "bookmark", new BsonDocument("$ne", new BsonArray()) }
            }))
            // Join
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
            // Filter by CategoryId
            .AppendStage<BsonDocument>(categoryIds != null && categoryIds.Any()
            ? new BsonDocument("$match", new BsonDocument
            {
                { "categories._id", new BsonDocument("$in", new BsonArray(categoryIds)) }
            })
            : new BsonDocument("$match", new BsonDocument()))
            .As<Domain.Models.Post>()
            .Sort(sortType.Equals("bookmarkedDate") ? new BsonDocument("bookmark.created_date", isSortAsc ? 1 : -1) : sortDefinition)
            // Filter
            .Match(filterExpression)
            .AppendStage<BsonDocument>(cursorBookMarkedDate != null ? new BsonDocument("$match", new BsonDocument
            {
                { "bookmark.created_date", new BsonDocument(isSortAsc ? "$gt" : "$lt", cursorBookMarkedDate) }
            }) : new BsonDocument("$match", new BsonDocument()))
            .Limit(pageSize + 1)
            // Add Field IsBookMarked and assign true value
            .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                new BsonDocument("is_bookmarked", true)
            ))
            // Check if User has liked post or not
            .AppendStage<BsonDocument>(new BsonDocument("$lookup", new BsonDocument
            {
                { "from", nameof(Domain.Models.Like) },
                { "let", new BsonDocument
                    {
                    { "postId", "$_id" },
                        { "userId", userId ?? ""}
                    }
                },
                { "pipeline", new BsonArray
                    {
                        new BsonDocument("$match", new BsonDocument("$expr",
                            new BsonDocument("$and", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray { "$post_id", "$$postId" }),
                                new BsonDocument("$eq", new BsonArray { "$user_id._id", "$$userId" })
                            })
                        ))
                    }
                },
                { "as", "likedpost" }
            }))
            .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                new BsonDocument("is_liked",
                    new BsonDocument("$gt", new BsonArray
                        {
                            new BsonDocument("$size", "$likedpost"),
                            0
                        }
                    )
                )
            ))
            .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                new BsonDocument("word_count",
                    new BsonDocument("$size", new BsonDocument("$split", new BsonArray {
                        "$content",
                        " "
                    }))
                )
            ))
            // Select fields
            .Project(postProjection)
            .As<PostResponse>()
            .ToListAsync(cancellationToken);
        return documents;
    }
}
