using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.FavouriteCategory;
using MediaService.Presentation.Extensions;
using MongoDB.Bson;
using MediaService.Contract.Attributes;

namespace MediaService.Presentation.Endpoints.V1;
public static class FavouriteCategoryEndpoint
{
    public const string ApiName = "favourite_categories";
    private const string BaseUrl = $"/media-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapFavouriteCategoryApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("", HandleUpdateFavouriteCategoryAsync).WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapGet("", HandleGetAllPostsFromFavouriteCategoryAsync).WithMetadata(new RequireRolesAttribute("Patient"));
        return builder;
    }

    private static async Task<IResult> HandleUpdateFavouriteCategoryAsync(ISender sender, IUserContext context,
        [FromBody] List<ObjectId> categoryIds)
    {
        var userId = context.UserId;
        var result = await sender.Send(new UpdateFavouriteCategoryCommand(categoryIds, userId ?? ""));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetAllPostsFromFavouriteCategoryAsync(ISender sender, IUserContext context)
    {
        var userId = context.UserId;
        var result = await sender.Send(new GetAllPostsFromFavouriteCategoryQuery(userId ?? ""));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
}