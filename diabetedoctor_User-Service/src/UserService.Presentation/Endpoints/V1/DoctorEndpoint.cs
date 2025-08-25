using UserService.Contract.Attributes;
using UserService.Contract.Common.Pagination;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Doctors.Commands;
using UserService.Contract.Services.Doctors.Filters;
using UserService.Contract.Services.Doctors.Queries;
using UserService.Presentation.Extensions;

namespace UserService.Presentation.Endpoints.V1;

public static class DoctorEndpoint
{
    public const string ApiName = "doctors";
    private const string BaseUrl = $"/user-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapDoctorApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        
        group.DisableAntiforgery();
        
        group.MapGet("", HandleGetAllDoctorsAsync)
            .WithMetadata(new RequireRolesAttribute("Patient", "HospitalStaff", "Moderator", "SystemAdmin"));
        group.MapGet("{doctorId}", HandleGetDoctorByIdAsync)
            .WithMetadata(new RequireRolesAttribute("Patient", "HospitalStaff", "SystemAdmin"));
        group.MapGet("profile", HandleGetDoctorProfileAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        group.MapPut("profile", HandleUpdateDoctorProfileAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        // Care Plan Template
        group.MapPost("{patientId}/template", HandleCreateCarePlanTemplateAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        group.MapGet("{patientId}/template", HandleGetAllCarePlanTemplatesAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        group.MapPut("{patientId}/template/{carePlanTemplateId}", HandleUpdateCarePlanTemplateAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        group.MapDelete("template/{carePlanTemplateId}", HandleDeleteCarePlanTemplateAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        
        // Care Plan Instance
        group.MapPost("{patientId}/careplan", HandleCreateCarePlanInstanceAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        group.MapGet("{patientId}/careplan", HandleGetCarePlanMeasurementSchedulesAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        group.MapPut("{patientId}/careplan/{carePlanTemplateId}", HandleUpdateCarePlanInstanceAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        group.MapDelete("careplan/{carePlanTemplateId}", HandleDeleteCarePlanInstanceAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));
        return builder;
    }
    
    private static async Task<IResult> HandleGetAllDoctorsAsync(ISender sender, IUserContext context, 
        [AsParameters] CursorPaginationRequest cursorPaginationRequest,
        [AsParameters] GetAllDoctorsFilter filters)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllDoctorsQuery
        {
            Pagination = cursorPaginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetDoctorByIdAsync(ISender sender, IUserContext context, 
        Guid doctorId)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetDoctorByIdQuery
        {
            DoctorId = doctorId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetDoctorProfileAsync(ISender sender, IUserContext context)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetDoctorByIdQuery
        {
            DoctorId = userId
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleUpdateDoctorProfileAsync(
        ISender sender,
        IUserContext context,
        [FromBody] UpdateDoctorProfileCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();        
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleCreateCarePlanTemplateAsync(
        ISender sender,
        IUserContext context,
        Guid patientId,
        [FromBody] CreateCarePlanTemplateCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { DoctorId = userId,  PatientId = patientId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleUpdateCarePlanTemplateAsync(
        ISender sender,
        IUserContext context,
        Guid carePlanTemplateId,
        Guid patientId,
        [FromBody] UpdateCarePlanTemplateCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { DoctorId = userId, PatientId = patientId, Id = carePlanTemplateId};
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetAllCarePlanTemplatesAsync(ISender sender, IUserContext context, 
        [AsParameters] CursorPaginationRequest cursorPaginationRequest,
        [AsParameters] GetAllCarePlanTemplatesFilter filters,
        Guid patientId)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();        
        var query = new GetAllCarePlanTemplatesQuery()
        {
            DoctorId = userId,
            PatientId = patientId,
            Pagination = cursorPaginationRequest,
            Filters = filters
        };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleDeleteCarePlanTemplateAsync(
        ISender sender,
        IUserContext context,
        Guid carePlanTemplateId)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();        
        var command = new DeleteCarePlanTemplateCommand { DoctorId = userId, Id = carePlanTemplateId};
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleCreateCarePlanInstanceAsync(
        ISender sender,
        IUserContext context,
        Guid patientId,
        [FromBody] CreateCarePlanInstanceCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { DoctorId = userId, PatientId = patientId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUpdateCarePlanInstanceAsync(
        ISender sender,
        IUserContext context,
        Guid patientId,
        Guid carePlanTemplateId,
        [FromBody] UpdateCarePlanInstanceCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { DoctorId = userId, PatientId = patientId, Id = carePlanTemplateId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleDeleteCarePlanInstanceAsync(
        ISender sender,
        IUserContext context,
        Guid carePlanTemplateId)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = new DeleteCarePlanInstanceCommand { DoctorId = userId, Id = carePlanTemplateId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetCarePlanMeasurementSchedulesAsync
    (ISender sender, IUserContext context, Guid patientId, [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();

        var result = await sender.Send(new GetCarePlanMeasurementQuery(patientId, userId, fromDate, toDate));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}
