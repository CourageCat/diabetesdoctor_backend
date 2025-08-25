using UserService.Contract.Attributes;
using UserService.Contract.Common.Pagination;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Hospitals.Commands;
using UserService.Contract.Services.Hospitals.Filteres;
using UserService.Contract.Services.Hospitals.Queries;
using UserService.Presentation.Extensions;

namespace UserService.Presentation.Endpoints.V1;

public static class HospitalEndpoint
{
    public const string ApiName = "hospitals";
    private const string BaseUrl = $"/user-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapHospitalApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);

        group.DisableAntiforgery();

        group.MapPost("doctors", HandleCreateDoctorAsync)
            .WithMetadata(new RequireRolesAttribute("HospitalStaff"));
        group.MapGet("me/doctors", HandleGetAllDoctorsByStaffAsync)
            .WithMetadata(new RequireRolesAttribute("HospitalStaff"));

        group.MapPost("", HandleCreateHospitalAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        group.MapPut("{hospitalId}", HandleUpdateHospitalAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        group.MapGet("me", HandleGetAllHospitalsByAdminAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin"));
        group.MapGet("", HandleGetAllHospitalsAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin", "Patient"));
        group.MapGet("{hospitalId}", HandleGetHospitalByIdAsync)
            .WithMetadata(new RequireRolesAttribute("SystemAdmin", "Patient"));

        group.MapPost("hospitalstaffs", HandleCreateHospitalStaffAsync)
            .WithMetadata(new RequireRolesAttribute("HospitalAdmin"));
        group.MapPut("hospitalstaffs/profile", HandleUpdateHospitalStaffAsync)
            .WithMetadata(new RequireRolesAttribute("HospitalStaff"));
        group.MapGet("me/hospitalstaffs", HandleGetAllHospitalStaffsByAdminAsync)
            .WithMetadata(new RequireRolesAttribute("HospitalAdmin"));
        group.MapGet("hospitalstaffs/{hospitalStaffId}", HandleGetHospitalStaffByIdAsync)
            .WithMetadata(new RequireRolesAttribute("HospitalAdmin"));
        group.MapGet("hospitalstaffs/profile", HandleGetHospitalStaffProfileAsync)
            .WithMetadata(new RequireRolesAttribute("HospitalStaff"));
        return builder;
    }

    private static async Task<IResult> HandleGetAllHospitalsAsync(ISender sender, IUserContext context,
        [AsParameters] CursorPaginationRequest cursorPaginationRequest,
        [AsParameters] GetAllHospitalsFilter filters)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllHospitalsQuery()
        {
            Pagination = cursorPaginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetHospitalByIdAsync(ISender sender, IUserContext context,
        Guid hospitalId)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetHospitalByIdQuery
        {
            HospitalId = hospitalId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateHospitalStaffAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateHospitalStaffCommand request)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { HospitalAdminId = userId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetAllHospitalStaffsByAdminAsync(ISender sender, IUserContext context,
        [AsParameters] OffsetPaginationRequest offsetPaginationRequest,
        [AsParameters] GetAllHospitalStaffsByAdminFilter filter)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllHospitalStaffsByAdminQuery()
        {
            HospitalAdminId = userId,
            Pagination = offsetPaginationRequest,
            Filters = filter
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateHospitalAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateHospitalCommand request)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { AdminId = userId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUpdateHospitalAsync(
        ISender sender,
        IUserContext context,
        Guid hospitalId,
        [FromBody] UpdateHospitalCommand request)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { Id = hospitalId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetAllHospitalsByAdminAsync(ISender sender, IUserContext context,
        [AsParameters] OffsetPaginationRequest offsetPaginationRequest,
        [AsParameters] GetAllHospitalsByAdminFilter filter)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllHospitalsByAdminQuery()
        {
            AdminId = userId,
            Pagination = offsetPaginationRequest,
            Filters = filter
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUpdateHospitalStaffAsync(
        ISender sender,
        IUserContext context,
        [FromBody] UpdateHospitalStaffCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateDoctorAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateDoctorCommand request)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { HospitalStaffId = userId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetAllDoctorsByStaffAsync(ISender sender, IUserContext context,
        [AsParameters] OffsetPaginationRequest offsetPaginationRequest,
        [AsParameters] GetAllDoctorsByStaffFilter filter)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllDoctorsByStaffQuery()
        {
            HospitalStaffId = userId,
            Pagination = offsetPaginationRequest,
            Filters = filter
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetHospitalStaffByIdAsync(ISender sender, IUserContext context,
        Guid hospitalStaffId)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetHospitalStaffByIdQuery()
        {
            HospitalStaffId = hospitalStaffId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetHospitalStaffProfileAsync(ISender sender, IUserContext context)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetHospitalStaffByIdQuery()
        {
            HospitalStaffId = userId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}