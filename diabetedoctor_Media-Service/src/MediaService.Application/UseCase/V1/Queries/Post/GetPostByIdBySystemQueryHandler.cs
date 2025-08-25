using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Services.Post;
using MediaService.Domain.Abstractions.Repositories;
using MediaService.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MediaService.Application.UseCase.V1.Queries.Post;
public sealed class GetPostByIdBySystemQueryHandler(IMongoDbContext context, IUnitOfWork unitOfWork) 
    : IQueryHandler<GetPostByIdBySystemQuery, Success<PostResponse>>
{
    public async Task<Result<Success<PostResponse>>> Handle(GetPostByIdBySystemQuery request, CancellationToken cancellationToken)
    {
        var postProjection = new BsonDocument()
        {
            { "_id", 1 },
            { "title", 1 },
            { "content", 1 },
            { "content_html", 1 },
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
            { "reason_rejected", 1 },
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
        var result = await context.Posts.Aggregate()
            // Filter
            .Match(x => x.Id == request.Id && x.IsDeleted == false)
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
            // Select fields
            .Project(postProjection)
            .As<PostResponse>()
            .FirstOrDefaultAsync(cancellationToken);
        if (result is null)
        {
            return Result.Failure<Success<PostResponse>>(PostErrors.PostNotFound);
        }
        return Result.Success(new Success<PostResponse>(
            PostMessage.GetPostByIdSuccessfully.GetMessage().Code,
            PostMessage.GetPostByIdSuccessfully.GetMessage().Message, result));
    }
}
