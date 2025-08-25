using MediaService.Contract.DTOs.CategoryDTOs;
using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.DTOs.UserDTOs;
using MediaService.Contract.Services.FavouriteCategory;
using MediaService.Contract.Services.FavouriteCategory.Responses;
using MediaService.Domain.Abstractions;
using MediaService.Domain.Models;
using MediaService.Domain.ValueObjects;
using MediaService.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;


namespace MediaService.Application.UseCase.V1.Queries.FavouriteCategory;

public sealed class GetAllPostsFromFavouriteCategoryQueryHandler(IMongoDbContext mongoDbContext)
    : IQueryHandler<GetAllPostsFromFavouriteCategoryQuery, Success<IEnumerable<FavouriteCategoryResponse>>>
{
    public async Task<Result<Success<IEnumerable<FavouriteCategoryResponse>>>> Handle(
        GetAllPostsFromFavouriteCategoryQuery request, CancellationToken cancellationToken)
    {
        var postProjection = new BsonDocument()
        {
            { "_id", 1 },
            { "title", 1 },
            { "thumbnail", "$thumbnail.public_url" },
            { "created_date", 1 },
            { "view", 1 },
            {
                "user", new BsonDocument
                {
                    { "_id", "$user.user_id._id" },
                    { "full_name", "$user.full_name" },
                    { "public_url", "$user.avatar.public_url" },
                }
            }
        };
        var pipeline = new EmptyPipelineDefinition<Domain.Models.Post>()
            .Match(new BsonDocument
                { { "$expr", new BsonDocument("$eq", new BsonArray { "$category_id", "$$categoryId" }) } })
            .Sort(Builders<Domain.Models.Post>.Sort.Descending(post => post.CreatedDate))
            .Limit(3)
            .Lookup<Domain.Models.Post, Domain.Models.Post, User, PostDto>(
                foreignCollection: mongoDbContext.Users,
                localField: post => post.ModeratorId,
                foreignField: user => user.UserId,
                @as: postDto => postDto.User)
            .Unwind(postDto => postDto.User, new AggregateUnwindOptions<BsonDocument>
            {
                PreserveNullAndEmptyArrays = true
            })
            .Project(postProjection)
            .As<Domain.Models.Post, BsonDocument, PostDto>();
        var filter = Builders<Domain.Models.FavouriteCategory>.Filter.Eq(fc => fc.UserId, UserId.Of(request.UserId));
        var result = await mongoDbContext.FavouriteCategories.Aggregate()
            .Match(filter)
            .Lookup(
                foreignCollectionName: nameof(Domain.Models.Category),
                localField: "category_id",
                foreignField: "_id",
                @as: "category")
            .Unwind("category", new AggregateUnwindOptions<BsonDocument>
            {
                PreserveNullAndEmptyArrays = true
            })
            .Lookup<Domain.Models.Post, PostDto, IEnumerable<PostDto>, FavouriteCategoryResponse>(
                foreignCollection: mongoDbContext.Posts,
                let: new BsonDocument("categoryId", "$category._id"),
                lookupPipeline: pipeline,
                @as: "posts"
            )
            .As<FavouriteCategoryResponse>()
            .ToListAsync(cancellationToken);
        return Result.Success(new Success<IEnumerable<FavouriteCategoryResponse>>(
            FavouriteCategoryMessage.GetAllPostsFromFavouriteCategorySuccessfully.GetMessage().Code,
            FavouriteCategoryMessage.GetAllPostsFromFavouriteCategorySuccessfully.GetMessage().Message, result));
    }
}