using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Services.Post;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MediaService.Application.UseCase.V1.Queries.Post;
public sealed class GetPostByIdQueryHandler(IMongoDbContext context, IPostRepository postRepository, IUnitOfWork unitOfWork) 
    : IQueryHandler<GetPostByIdQuery, Success<PostResponse>>
{
    public async Task<Result<Success<PostResponse>>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await GetPostAsync(request.Id, request.UserId, cancellationToken);
        if (result is null)
        {
            return Result.Failure<Success<PostResponse>>(PostErrors.PostNotFound);
        }
        await UpdateViewPost(request.Id, cancellationToken);
        return Result.Success(new Success<PostResponse>(
            PostMessage.GetPostByIdSuccessfully.GetMessage().Code,
            PostMessage.GetPostByIdSuccessfully.GetMessage().Message, result));
    }

    private async Task<PostResponse?> GetPostAsync(ObjectId id, string? userId, CancellationToken cancellationToken)
    {
        var postProjection = new BsonDocument()
        {
            { "_id", 1 },
            { "title", 1 },
            { "content", 1 },
            { "content_html", 1 },
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
        var document = await context.Posts.Aggregate()
            // Filter
            .Match(x => x.Id == id && x.Status == PostStatus.Approved && x.IsDeleted == false)
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
                        { "userId", userId ?? "" }
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
            .FirstOrDefaultAsync(cancellationToken);
        return document;
    }

    private async Task UpdateViewPost(ObjectId id, CancellationToken cancellationToken)
    {
        // Update View for Post
        var postFound = await postRepository.ExistAsync(post => post.Id == id, cancellationToken);
        if (!postFound)
        {
            return;
        }
        await unitOfWork.StartTransactionAsync(cancellationToken);
        try
        {
            await postRepository.UpdateViewPost(unitOfWork.ClientSession, id, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
    }
}
