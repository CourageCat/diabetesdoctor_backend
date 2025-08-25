using MediaService.Contract.DTOs.CategoryDTOs;
using MediaService.Contract.DTOs.PostDTOs;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.Category;
using MediaService.Contract.Services.Post;
using MediaService.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MediaService.Contract.Attributes;
using MediaService.Contract.Common.Filters;
using MediaService.Contract.DTOs.PostDTOs.GetAllPostsRequestDTO;
using MediaService.Contract.Enumarations.User;
using Filter = MediaService.Contract.Services.Category.Filter;

namespace MediaService.Presentation.Endpoints.V1;
public static class CategoryEndpoint
{
    public const string ApiName = "categories";
    private const string BaseUrl = $"/media-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapCategoryApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("", HandleCreateCategoryAsync).WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        group.MapGet("", HandleGetAllCategoriesAsync).WithMetadata(new RequireRolesAttribute("Patient", "Moderator", "SystemAdmin", "Doctor"));
        group.MapGet("/admin", HandleGetAllCategoriesByAdminAsync).WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        group.MapGet("{id}", HandleGetCategoryByIdAsync).WithMetadata(new RequireRolesAttribute());
        group.MapGet("/top-post", HandleGetTopPostCategoriesAsync).WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        //group.MapDelete("delete-category", HandleDeleteCategoryAsync);
        //group.MapPut("", HandleUpdateCategoryAsync);
        return builder;
    }

    private static async Task<IResult> HandleCreateCategoryAsync(ISender sender, IUserContext context, 
        [FromBody] CreateCategoryCommand request)
    {
        var result = await sender.Send(request);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    private static async Task<IResult> HandleGetAllCategoriesAsync(ISender sender, IUserContext context)
    {
        var userRole = context.Role;
        var userAddedToFavouriteId = context.UserId ?? "";
        var result = await sender.Send(new GetAllCategoriesQuery(userRole == nameof(RoleType.Patient) ? userAddedToFavouriteId : ""));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetAllCategoriesByAdminAsync(ISender sender, [AsParameters] GetAllCategoriesRequestDto request,
        [AsParameters] QueryPageIndex filter)
    {
        var categoryFilter = new Filter.CategoryFilter(request.SearchContent);
        var result = await sender.Send(new GetAllCategoriesByAdminQuery(filter.PageIndex ?? 1, filter.PageSize ?? 10, categoryFilter, filter.SortType ?? "name", filter.IsSortAsc ?? false));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetCategoryByIdAsync(ISender sender, ObjectId id)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetTopPostCategoriesAsync(ISender sender, IUserContext context, [AsParameters] GetTopPostCategoriesRequestDto request)
    {
        var userAddedToFavouriteId = context.UserId;
        var result = await sender.Send(new GetTopPostCategoriesQuery(userAddedToFavouriteId ?? "", request.NumberOfCategories));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleDeleteCategoryAsync(ISender sender, [FromQuery] ObjectId id)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUpdateCategoryAsync(ISender sender, [FromQuery] ObjectId id, [FromForm] UpdateCategoryRequestDto updateDto)
    {
        var updateCommand = new UpdateCategoryCommand(id, updateDto.Name, updateDto.Description, updateDto.Image);
        var result = await sender.Send(updateCommand);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}