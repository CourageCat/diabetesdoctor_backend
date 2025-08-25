using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Services.Post;
using MediaService.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
namespace MediaService.Application.UseCase.V1.Queries.Post;
public sealed class GetTopViewPostsQueryHandler(IMongoDbContext mongoDbContext) : IQueryHandler<GetTopViewPostsQuery, Success<IEnumerable<PostResponse>>>
{
    public async Task<Result<Success<IEnumerable<PostResponse>>>> Handle(GetTopViewPostsQuery request, CancellationToken cancellationToken)
    {
        // Subtracting a number of days from the current date
        var targetDate = DateTime.Now.AddDays(-request.NumberOfDays);
        // Get Top Posts from Date Interval
        var recentPosts = await GetPostsAsync(request.UserId, request.NumberOfPosts, targetDate, DateTime.Now, cancellationToken);
        var result = recentPosts.ToList();
        // If Number of Top Posts < number of Posts in request => Get the remain from Date < Date Interval
        if (recentPosts.ToList().Count < request.NumberOfPosts)
        {
            var numberOfPostsRemain = request.NumberOfPosts - recentPosts.ToList().Count;
            var fallbackPosts = await GetPostsAsync(request.UserId, numberOfPostsRemain, DateTime.MinValue, targetDate, cancellationToken);
            result.AddRange(fallbackPosts.ToList());
        }
        result = result.OrderByDescending(post => post.View).ToList();
        return Result.Success(new Success<IEnumerable<PostResponse>>(
            PostMessage.GetTopViewPostsSuccessfully.GetMessage().Code,
            PostMessage.GetTopViewPostsSuccessfully.GetMessage().Message, result));
    }

    private async Task<IEnumerable<PostResponse>> GetPostsAsync(string? userBookMarkedId, int numberOfPosts, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
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
            {
                "doctor", new BsonDocument
                {
                    { "_id", "$doctor.user_id._id" },
                    { "full_name", "$doctor.full_name" },
                    { "public_url", "$doctor.avatar.public_url" },
                }
            },
            { "status", 1 },
            { "is_bookmarked", 1 },
            { "is_liked", 1 },
            { "word_count", 1 }
        };
        var categoryProjection = new BsonDocument()
        {
            { "_id", "$category._id" },
            { "name", "$category.name" },
            { "image_url", "$category.image.public_url" }
        };
        var lookupPipeline = new EmptyPipelineDefinition<Domain.Models.PostCategory>()
            .Match(new BsonDocument
                { { "$expr", new BsonDocument("$eq", new BsonArray { "$post_id", "$$postId" }) } })
            .Sort(Builders<Domain.Models.PostCategory>.Sort.Descending(postCategory => postCategory.Id))
            .Lookup<Domain.Models.PostCategory, Domain.Models.PostCategory, Domain.Models.Category, PostCategoryDto>(
                foreignCollection: mongoDbContext.Categories,
                localField: postCategory => postCategory.CategoryId,
                foreignField: category => category.Id,
                @as: postCategoryDto => postCategoryDto.Category)
            .Unwind(postCategory => postCategory.Category, new AggregateUnwindOptions<BsonDocument>
            {
                PreserveNullAndEmptyArrays = true
            })
            .Project(categoryProjection)
            .As<Domain.Models.PostCategory, BsonDocument, Domain.Models.Category>();
        var documents = await mongoDbContext.Posts.Aggregate()
            .Match(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate && x.Status == PostStatus.Approved && x.IsDeleted == false)
            .Sort(Builders<Domain.Models.Post>.Sort.Descending(x => x.View))
            .Limit(numberOfPosts)
            .Lookup<Domain.Models.PostCategory, Domain.Models.Category, IEnumerable<Domain.Models.Category>, PostResponse>(
                foreignCollection: mongoDbContext.PostCategories,
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
            // Check if User has Add Post To BookMark or not
            .AppendStage<BsonDocument>(new BsonDocument("$lookup", new BsonDocument
            {
                { "from", nameof(Domain.Models.BookMark) },
                { "let", new BsonDocument
                    {
                    { "postId", "$_id" },
                        { "userId", userBookMarkedId != null ? userBookMarkedId : "" }
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
                { "as", "bookmark" }
            }))
            .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                new BsonDocument("is_bookmarked",
                    new BsonDocument("$gt", new BsonArray
                        { new BsonDocument("$size", "$bookmark"),0 }
                    )
                )
            ))
            // Check if User has liked post or not
            .AppendStage<BsonDocument>(new BsonDocument("$lookup", new BsonDocument
            {
                { "from", nameof(Domain.Models.Like) },
                { "let", new BsonDocument
                    {
                    { "postId", "$_id" },
                        { "userId", userBookMarkedId ?? ""}
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
            .Project(postProjection)
            .As<PostResponse>()
            .ToListAsync(cancellationToken);
        return documents;
    }
}
