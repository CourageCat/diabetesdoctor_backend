using MediaService.Contract.Common.DomainErrors;
using MediaService.Contract.Services.Category;
using MediaService.Domain.Abstractions;
using MediaService.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MediaService.Application.UseCase.V1.Queries.Category;

public sealed class GetCategoryByIdQueryHandler(IMongoDbContext context) : IQueryHandler<GetCategoryByIdQuery, Success<CategoryResponse>>
{
    public async Task<Result<Success<CategoryResponse>>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var categoryProjection = new BsonDocument()
        {
            { "_id", 1 },
            { "name", 1 },
            { "description", 1 },
            { "image_url", "$image.public_url" },
            { "created_date", 1 },
            { "number_of_posts", 1 }
        };
        var result = await context.Categories.Aggregate()
            .Match(c => c.Id == request.Id && c.IsDeleted == false)
            .Sort(Builders<Domain.Models.Category>.Sort.Descending(c => c.CreatedDate))
            .Lookup(
                foreignCollectionName: nameof(Domain.Models.PostCategory),
                localField: "_id",
                foreignField: "category_id",
                @as: "postCategories"
            )
            // Count number of posts of each Category
            .AppendStage<BsonDocument>(new BsonDocument("$addFields",
                new BsonDocument("number_of_posts",
                    new BsonDocument("$size", "$postCategories")
                )
            ))
            .As<CategoryResponse>()
            .FirstOrDefaultAsync(cancellationToken);
        if (result is null)
        {
            return Result.Failure<Success<CategoryResponse>>(CategoryErrors.CategoryNotFoundException);
        }
        return Result.Success(new Success<CategoryResponse>(
            CategoryMessage.GetCategoryByIdSuccessfully.GetMessage().Code,
            CategoryMessage.GetCategoryByIdSuccessfully.GetMessage().Message, result));
    }
}
