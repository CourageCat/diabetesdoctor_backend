using UserService.Contract.Attributes;
using UserService.Contract.Common.Pagination;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.ServicePackages.Commands;
using UserService.Contract.Services.ServicePackages.Filters;
using UserService.Contract.Services.ServicePackages.Queries;
using UserService.Presentation.Extensions;

namespace UserService.Presentation.Endpoints.V1;

public static class ServicePackageEndpoint
{
    public const string ApiName = "service_packages";
    private const string BaseUrl = $"/user-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapServicePackageApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("", HandleCreateServicePackageAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        group.MapGet("", HandleGetAllServicePackagesAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin", "Patient", "Doctor"));
        group.MapGet("/by-admin", HandleGetAllServicePackagesByAdminAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        group.MapGet("/purchased", HandleGetAllServicePackagesPurchasedAsync)
            .WithMetadata(new RequireRolesAttribute("Patient", "Doctor"));
        group.MapGet("{servicePackageId}", HandleGetServicePackageByIdAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin", "Patient", "Doctor"));

        return builder;
    }

    private static async Task<IResult> HandleCreateServicePackageAsync(ISender sender, IUserContext context,
        [FromBody] CreateServicePackageCommand request)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { AdminId = userId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetAllServicePackagesAsync(ISender sender, IUserContext context,
        [AsParameters] CursorPaginationRequest cursorPaginationRequest,
        [AsParameters] GetAllServicePackagesFilter filters)
    {
        var query = new GetAllServicePackagesQuery()
        {
            Pagination = cursorPaginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetAllServicePackagesByAdminAsync(ISender sender, IUserContext context,
        [AsParameters] OffsetPaginationRequest offsetPaginationRequest,
        [AsParameters] GetAllServicePackagesByAdminFilter filters)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllServicePackagesByAdminQuery()
        {
            Pagination = offsetPaginationRequest,
            Filters = filters,
            AdminId = userId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetAllServicePackagesPurchasedAsync(ISender sender, IUserContext context,
        [AsParameters] CursorPaginationRequest offsetPaginationRequest,
        [AsParameters] GetAllServicePackagesPurchasedFilter filters)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllServicePackagesPurchasedQuery()
        {
            Pagination = offsetPaginationRequest,
            Filters = filters,
            UserId = userId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetServicePackageByIdAsync(ISender sender, IUserContext context,
        Guid servicePackageId)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetServicePackageByIdQuery()
        {
            ServicePackageId = servicePackageId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}