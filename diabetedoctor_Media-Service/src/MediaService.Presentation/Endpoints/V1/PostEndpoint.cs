using MediaService.Contract.Common;
using MediaService.Contract.Common.Filters;
using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.DTOs.PostDTOs.GetAllPostsRequestDTO;
using MediaService.Contract.Enumarations.Post;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.BookMark;
using MediaService.Contract.Services.Post;
using MediaService.Presentation.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MediaService.Contract.Attributes;
using MediaService.Contract.Enumarations.User;
using static MediaService.Contract.Services.Post.Filter;
namespace MediaService.Presentation.Endpoints.V1;

public static class PostEndpoint
{
    public const string ApiName = "posts";
    private const string BaseUrl = $"/media-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapPostApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("", HandleCreateDraftPostAsync).WithMetadata(new RequireRolesAttribute("Moderator"));
        group.MapPut("{id}", HandleUpdatePostAsync).WithMetadata(new RequireRolesAttribute("Moderator"));
        group.MapPut("/review/{id}", HandleReviewPostAsync).WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        group.MapDelete("{id}", HandleDeletePostAsync).WithMetadata(new RequireRolesAttribute("Moderator"));
        group.MapGet("", HandleGetAllPostsAsync).WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        group.MapGet("/system", HandleGetAllPostsBySystemAsync).WithMetadata(new RequireRolesAttribute("Moderator", "SystemAdmin"));
        group.MapGet("{id}", HandleGetPostByIdAsync).WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        group.MapGet("/system/{id}", HandleGetPostByIdBySystemAsync).WithMetadata(new RequireRolesAttribute("Moderator", "SystemAdmin"));
        group.MapGet("/top-view", HandleGetTopViewPostsAsync).WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        group.MapGet("/like", HandleGetAllLikePostsAsync).WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        group.MapGet("/bookmark", HandleGetAllPostsFromBookMarkAsync).WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));

        return builder;
    }
    private static async Task<IResult> HandleCreateDraftPostAsync(ISender sender, IUserContext context)
    {
        var userId = context.UserId;
        var createCommand = new CreateDraftPostCommand(userId ?? "");
        var result = await sender.Send(createCommand);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    private static async Task<IResult> HandleUpdatePostAsync(ISender sender, IUserContext context, ObjectId id, [FromBody]UpdatePostRequestDto request)
    {
        var userId = context.UserId;
        var updateCommand = new UpdatePostCommand(id, request.Title, request.Content, request.ContentHtml, request.Thumbnail,  request.CategoryIds, request.Images, userId ?? "", request.DoctorId, request.IsDraft);
        var result = await sender.Send(updateCommand);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    private static async Task<IResult> HandleReviewPostAsync(ISender sender, IUserContext context, ObjectId id, [FromBody] ReviewPostRequestDto request)
    {
        var result = await sender.Send(new ReviewPostCommand(id, request.IsApproved, request.ReasonRejected));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    private static async Task<IResult> HandleDeletePostAsync(ISender sender, IUserContext context, ObjectId id)
    {
        var userId = context.UserId;
        var result = await sender.Send(new DeletePostCommand(id, userId ?? ""));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    private static async Task<IResult> HandleGetAllPostsAsync(ISender sender, IUserContext context, [AsParameters] GetAllPostsByUserRequestDto request,
    [AsParameters] QueryFilter paging)
    {
        var userBookMarkedId = context.UserId;
        var filterParams = new PostFilter(request.SearchContent, request.CategoryIds, Status.Approved, request.TutorialType, userBookMarkedId, request.ModeratorId, request.DoctorId);
        var result = await sender.Send(new GetAllPostsQuery(paging.Cursor, paging.PageSize ?? 10, filterParams, paging.SortType ?? "createdDate", paging.IsSortAsc ?? false));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetAllPostsBySystemAsync(ISender sender, IUserContext context, [AsParameters] GetAllPostsBySystemRequestDto request,
        [AsParameters] QueryPageIndex paging)
    {
        var userRole = context.Role;
        var userId = context.UserId;
        var filterParams = new PostFilter(request.SearchContent, request.CategoryIds, request.Status, null, null, userRole == nameof(RoleType.SystemAdmin) ? request.ModeratorId : userId, request.DoctorId);
        var result = await sender.Send(new GetAllPostsBySystemQuery(paging.PageIndex ?? 1, paging.PageSize ?? 10, filterParams, paging.SortType ?? "createdDate", paging.IsSortAsc ?? false));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetPostByIdAsync(ISender sender, IUserContext context, ObjectId id)
    {
        var userBookMarkedId = context.UserId;
        var result = await sender.Send(new GetPostByIdQuery(id, userBookMarkedId));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    private static async Task<IResult> HandleGetPostByIdBySystemAsync(ISender sender, IUserContext context, ObjectId id)
    {
        var result = await sender.Send(new GetPostByIdBySystemQuery(id));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetTopViewPostsAsync(ISender sender, IUserContext context, [AsParameters] GetTopViewPostsRequestDto request)
    {
        var userBookMarkedId = context.UserId;
        var result = await sender.Send(new GetTopViewPostsQuery(userBookMarkedId, request.NumberOfPosts, request.NumberOfDays));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();

    }

    private static async Task<IResult> HandleGetAllLikePostsAsync(ISender sender, IUserContext context, [AsParameters] GetAllPostsByUserRequestDto request,
    [AsParameters] QueryFilter paging)
    {
        var userId = context.UserId;
        var filterParams = new PostFilter(request.SearchContent, request.CategoryIds, Status.Approved, null, userId, request.ModeratorId, request.DoctorId);
        var result = await sender.Send(new GetAllLikePostsQuery(paging.Cursor, paging.PageSize ?? 10, filterParams, paging.SortType ?? "createdDate", paging.IsSortAsc ?? false));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetAllPostsFromBookMarkAsync(ISender sender, IUserContext context, [AsParameters] GetAllPostsByUserRequestDto request,
    [AsParameters] QueryFilter paging)
    {
        var userId = context.UserId;
        var filterParams = new PostFilter(request.SearchContent, request.CategoryIds, Status.Approved, null, userId, request.ModeratorId, request.DoctorId);
        var result = await sender.Send(new GetAllPostsFromBookMarkQuery(paging.Cursor, paging.PageSize ?? 10, filterParams, paging.SortType ?? "createdDate", paging.IsSortAsc ?? false));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}