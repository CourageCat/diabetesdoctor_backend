using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.Like;
using MediaService.Presentation.Extensions;
using MongoDB.Bson;
using MediaService.Contract.Attributes;

namespace MediaService.Presentation.Endpoints.V1;
public static class LikeEndpoint
{
    public const string ApiName = "likes";
    private const string BaseUrl = $"/media-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapLikeApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("{postId}", HandleLikePostAsync).WithMetadata(new RequireRolesAttribute("Patient"));
        return builder;
    }
    private static async Task<IResult> HandleLikePostAsync(ISender sender, IUserContext context, ObjectId postId)
    {
        var userLikedId = context.UserId;
        var result = await sender.Send(new LikePostCommand(userLikedId ?? "", postId));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}