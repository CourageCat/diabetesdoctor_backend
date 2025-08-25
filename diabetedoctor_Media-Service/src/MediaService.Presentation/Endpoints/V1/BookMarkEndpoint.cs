using MediaService.Contract.Common;
using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.BookMark;
using MediaService.Contract.Services.Post;
using MediaService.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MediaService.Contract.Attributes;
using static MediaService.Contract.Services.Post.Filter;
namespace MediaService.Presentation.Endpoints.V1;
public static class BookMarkEndpoint
{
    public const string ApiName = "bookmarks";
    private const string BaseUrl = $"/media-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapBookMarkApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("{postId}", HandleUpdateBookMarkAsync).WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapDelete("", HandleRemoveAllPostsFromBookMarkAsync).WithMetadata(new RequireRolesAttribute("Patient"));
        return builder;
    }
    private static async Task<IResult> HandleUpdateBookMarkAsync(ISender sender, IUserContext context,
        ObjectId postId)
    {
        var userId = context.UserId;
        var result = await sender.Send(new UpdateBookMarkCommand(postId, userId ?? ""));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    private static async Task<IResult> HandleRemoveAllPostsFromBookMarkAsync(ISender sender, IUserContext context)
    {
        var userId = context.UserId;
        var result = await sender.Send(new RemoveAllPostsFromBookMarkCommand(userId ?? ""));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }  
}