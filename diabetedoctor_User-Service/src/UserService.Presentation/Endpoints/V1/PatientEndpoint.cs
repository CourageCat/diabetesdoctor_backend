using System.Text.RegularExpressions;
using UserService.Contract.Attributes;
using UserService.Contract.Common.Pagination;
using UserService.Contract.DTOs;
using UserService.Contract.Enums;
using UserService.Contract.Infrastructure;
using UserService.Contract.Services.Patients.Commands;
using UserService.Contract.Services.Patients.Filters;
using UserService.Contract.Services.Patients.Queries;
using UserService.Presentation.Extensions;

namespace UserService.Presentation.Endpoints.V1;

public static class PatientEndpoint
{
    public const string ApiName = "users";
    private const string BaseUrl = $"/user-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapPatientApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);

        group.DisableAntiforgery();

        // Profile
        group.MapPost("/patients", HandleCreatePatientProfileAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapPut("/patients/profile", HandleUpdatePatientProfileAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapGet("/patients/profile", HandleGetPatientProfileAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapGet("/patients/profile/{patientId}", HandleGetPatientProfileByDoctorAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));

        // Health Record
        group.MapPost("/patients/records/weight", HandleCreateWeightValueAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapPost("/patients/records/height", HandleCreateHeightValueAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapPost("/patients/records/blood-pressure", HandleCreateBloodPressureValueAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapPost("/patients/records/blood-glucose", HandleCreateBloodGlucoseAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapPost("/patients/records/hbA1c", HandleCreateHbA1cGlucoseAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapGet("/patients/records", HandleGetHealthRecordsAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapGet("/patients/records/{patientId}", HandleGetHealthRecordsByDoctorAsync)
            .WithMetadata(new RequireRolesAttribute("Doctor"));

        // Care Plan Template
        group.MapPost("/patients/template", HandleCreateCarePlanTemplateAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapGet("/patients/template", HandleGetAllCarePlanTemplatesAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapPut("/patients/template/{carePlanTemplateId}", HandleUpdateCarePlanTemplateAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapDelete("/patients/template/{carePlanTemplateId}", HandleDeleteCarePlanTemplateAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapGet("/patients/template/doctor-created", HandleGetAllDoctorCreatedTemplateAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));

        // Care Plan Instance
        group.MapPost("/patients/careplan", HandleCreateCarePlanInstanceAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapGet("/patients/careplan", HandleGetCarePlanMeasurementSchedulesAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapPut("/patients/careplan/{carePlanTemplateId}", HandleUpdateCarePlanInstanceAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));
        group.MapDelete("/patients/careplan/{carePlanTemplateId}", HandleDeleteCarePlanInstanceAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));

        // Avatar
        group.MapPatch("/avatar", HandleChangeAvatarAsync)
            .WithMetadata(new RequireRolesAttribute("Patient", "Doctor", "Moderator", "SystemAdmin", "HospitalStaff"));
        group.MapPatch("/patients/records/ai-note/{healthRecordId}", HandleUpdateAiNoteAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));

        // Consultation
        group.MapGet("/patients/consultation-sessions", HandleGetConsultationSessionsAsync)
            .WithMetadata(new RequireRolesAttribute("Patient"));

        // Media
        group.MapPost("/media", HandleUploadFilesAsync).WithMetadata(
            new RequireRolesAttribute("Patient", "SystemAdmin", "Moderator", "HospitalStaff", "HospitalAdmin"));
        group.MapDelete("/media", HandleDeleteFilesAsync);

        return builder;
    }

    private static async Task<IResult> HandleCreatePatientProfileAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreatePatientProfileCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUpdatePatientProfileAsync(
        ISender sender,
        IUserContext context,
        [FromBody] UpdatePatientProfileCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateWeightValueAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateWeightValueCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateHeightValueAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateHeightValueCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateBloodPressureValueAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateBloodPressureValueCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateBloodGlucoseAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateBloodGlucoseValueCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateHbA1cGlucoseAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateHbA1cValueCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { UserId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetHealthRecordsAsync(
        ISender sender,
        IUserContext context,
        [FromQuery] string recordTypes,
        [FromQuery] bool newest = true,
        [FromQuery] bool isBelongToDoctorTemplate = false,
        [FromQuery] string? fromDate = null,
        [FromQuery] string? toDate = null,
        [FromQuery] bool onePerType = true
    )
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();

        var fromDateParse = ParseDateTimeOffset(fromDate);
        var toDateParse = ParseDateTimeOffset(toDate);

        var parsedRecordTypes = recordTypes
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(rt => rt.Trim())
            .Select(rt => Enum.TryParse<RecordEnum>(rt, true, out var result) ? (RecordEnum?)result : null)
            .Where(rt => rt != null)
            .Select(rt => rt.Value)
            .ToList();

        var result = await sender.Send(new GetHealthRecordValuesQuery(userId, parsedRecordTypes, newest, isBelongToDoctorTemplate, fromDateParse,
            toDateParse, onePerType));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetHealthRecordsByDoctorAsync(
        ISender sender,
        IUserContext context,
        Guid patientId,
        [FromQuery] string recordTypes,
        [FromQuery] bool newest = true,
        [FromQuery] bool isBelongToDoctorTemplate = false,
        [FromQuery] string? fromDate = null,
        [FromQuery] string? toDate = null,
        [FromQuery] bool onePerType = true
    )
    {
        var fromDateParse = ParseDateTimeOffset(fromDate);
        var toDateParse = ParseDateTimeOffset(toDate);

        var parsedRecordTypes = recordTypes
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(rt => rt.Trim())
            .Select(rt => Enum.TryParse<RecordEnum>(rt, true, out var result) ? (RecordEnum?)result : null)
            .Where(rt => rt != null)
            .Select(rt => rt.Value)
            .ToList();

        var result = await sender.Send(new GetHealthRecordValuesQuery(patientId, parsedRecordTypes, newest, isBelongToDoctorTemplate,
            fromDateParse, toDateParse, onePerType));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetPatientProfileAsync(
        ISender sender,
        IUserContext context)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();

        var result = await sender.Send(new GetPatientProfileQuery(userId));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetPatientProfileByDoctorAsync(
        ISender sender,
        IUserContext context,
        Guid patientId)
    {
        var result = await sender.Send(new GetPatientProfileQuery(patientId));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetCarePlanMeasurementSchedulesAsync
    (ISender sender, IUserContext context, [FromQuery] Guid? doctorId, [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();

        var result = await sender.Send(new GetCarePlanMeasurementQuery(userId, doctorId, fromDate, toDate));
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateCarePlanTemplateAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateCarePlanTemplateCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { PatientId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUpdateCarePlanTemplateAsync(
        ISender sender,
        IUserContext context,
        Guid carePlanTemplateId,
        [FromBody] UpdateCarePlanTemplateCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { PatientId = userId, Id = carePlanTemplateId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetAllCarePlanTemplatesAsync(ISender sender, IUserContext context,
        [AsParameters] CursorPaginationRequest cursorPaginationRequest,
        [AsParameters] GetAllCarePlanTemplatesFilter filters)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllCarePlanTemplatesQuery()
        {
            PatientId = userId,
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
        var command = new DeleteCarePlanTemplateCommand { PatientId = userId, Id = carePlanTemplateId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleCreateCarePlanInstanceAsync(
        ISender sender,
        IUserContext context,
        [FromBody] CreateCarePlanInstanceCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { PatientId = userId };

        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUpdateCarePlanInstanceAsync(
        ISender sender,
        IUserContext context,
        Guid carePlanTemplateId,
        [FromBody] UpdateCarePlanInstanceCommand request)
    {
        // Sửa lại
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = request with { PatientId = userId, Id = carePlanTemplateId };
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
        var command = new DeleteCarePlanInstanceCommand { PatientId = userId, Id = carePlanTemplateId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUploadFilesAsync(ISender sender, IUserContext context,
        [FromForm] UploadImagesRequestDTO request)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var result = await sender.Send(new UploadFilesCommand { Images = request.Images, UploadedBy = userIdHeader });
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleDeleteFilesAsync(ISender sender, IUserContext context,
        [FromBody] List<string> imageIds)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var result = await sender.Send(new DeleteFilesCommand { ImageIds = imageIds });
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }


    private static DateTimeOffset? ParseDateTimeOffset(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var normalized = input.Replace('T', ' ');
        normalized = Regex.Replace(normalized, @"([+-]\d{2})(\d{2})$", "$1:$2");
        if (DateTimeOffset.TryParse(normalized, out var dto))
            return dto;

        return null;
    }

    private static async Task<IResult> HandleChangeAvatarAsync(ISender sender, IUserContext context,
        [FromForm] ChangeAvatarCommand request)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var roleHeader = context.Role;
        if (string.IsNullOrWhiteSpace(roleHeader))
            return Results.Unauthorized();
        var command = request with { UserId = userId, Role = roleHeader };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleUpdateAiNoteAsync(ISender sender, IUserContext context,
        Guid healthRecordId)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = new UpdateAiNoteCommand { HealthRecordId = healthRecordId, UserId = userId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }

    private static async Task<IResult> HandleGetConsultationSessionsAsync(ISender sender, IUserContext context)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var command = new GetConsultationSessionsQuery { UserId = userId };
        var result = await sender.Send(command);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleGetAllDoctorCreatedTemplateAsync(ISender sender, IUserContext context)
    {
        var userIdHeader = context.UserId;
        if (string.IsNullOrWhiteSpace(userIdHeader) || !Guid.TryParse(userIdHeader, out var userId))
            return Results.Unauthorized();
        var query = new GetAllDoctorCreatedTemplateQuery { PatientId = userId };
        var result = await sender.Send(query);
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}