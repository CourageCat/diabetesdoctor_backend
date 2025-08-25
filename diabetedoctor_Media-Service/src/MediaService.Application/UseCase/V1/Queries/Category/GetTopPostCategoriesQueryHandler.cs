using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Services.Category;

using MediaService.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MediaService.Application.UseCase.V1.Queries.Category;

public sealed class GetTopPostCategoriesQueryHandler(IMongoDbContext context) : IQueryHandler<GetTopPostCategoriesQuery, Success<IEnumerable<CategoryResponse>>>
{
    public async Task<Result<Success<IEnumerable<CategoryResponse>>>> Handle(GetTopPostCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categoryProjection = new BsonDocument()
        {
            { "_id", 1 },
            { "name", 1 },
            { "image_url", "$image.public_url" },
            { "created_date", 1 },
            { "is_added_to_favourite", "$is_added_to_favourite" },
            { "number_of_posts", 1 }
        };
        var result = await context.Categories.Aggregate()
            .Match(c => c.IsDeleted == false)
            .Lookup(
                foreignCollectionName: nameof(Domain.Models.PostCategory),
                localField: "_id",
                foreignField: "category_id",
                @as: "postCategories"
            )
            // Check if User has Add Category To Favourite or not
            .AppendStage<BsonDocument>(new BsonDocument("$lookup", new BsonDocument
            {
                { "from", nameof(Domain.Models.FavouriteCategory) },
                { "let", new BsonDocument
                    {
                    { "categoryId", "$_id" },
                        { "userId", request.UserId}
                    }
                },
                { "pipeline", new BsonArray
                    {
                        new BsonDocument("$match", new BsonDocument("$expr",
                            new BsonDocument("$and", new BsonArray
                            {
                                new BsonDocument("$eq", new BsonArray { "$category_id", "$$categoryId" }),
                                new BsonDocument("$eq", new BsonArray { "$user_id._id", "$$userId" })
                            })
                        ))
                    }
                },
                { "as", "favouriteCategory" }
            }))
            .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                new BsonDocument("is_added_to_favourite",
                    new BsonDocument("$gt", new BsonArray
                        { new BsonDocument("$size", "$favouriteCategory"),0 }
                    )
                )
            ))
            // Count number of posts of each Category
            .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                new BsonDocument("number_of_posts",
                    new BsonDocument("$size", "$postCategories")
                )
            ))
            .Sort(new BsonDocument("number_of_posts", -1))
            .Limit(request.NumberOfCategories)
            .Project(categoryProjection)
            .As<CategoryResponse>()
            .ToListAsync(cancellationToken);
        return Result.Success(new Success<IEnumerable<CategoryResponse>>(
            CategoryMessage.GetTopPostCategoriesSuccessfully.GetMessage().Code,
            CategoryMessage.GetTopPostCategoriesSuccessfully.GetMessage().Message, result));
    }
}
