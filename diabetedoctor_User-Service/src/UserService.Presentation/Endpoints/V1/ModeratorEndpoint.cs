using UserService.Contract.Attributes;
using UserService.Contract.Common.Pagination;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Moderators.Filters;
using UserService.Contract.Services.Moderators.Queries;
using UserService.Presentation.Extensions;

namespace UserService.Presentation.Endpoints.V1;

public static class ModeratorEndpoint
{
    public const string ApiName = "moderators";
    private const string BaseUrl = $"/user-service/api/v{{version:apiVersion}}/{ApiName}";
    
    public static IVersionedEndpointRouteBuilder MapModeratorApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();
        
        group.MapGet("", HandleGetAllModeratorsAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        
        return builder;
    }
    
    private static async Task<IResult> HandleGetAllModeratorsAsync(ISender sender, IUserContext context, 
        [AsParameters] CursorPaginationRequest cursorPaginationRequest,
        [AsParameters] GetAllModeratorsFilter filters)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();        var query = new GetAllModeratorsQuery()
        {
            Pagination = cursorPaginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}