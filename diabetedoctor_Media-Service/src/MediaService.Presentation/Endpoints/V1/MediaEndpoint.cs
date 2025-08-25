using MediaService.Contract.Attributes;
using MediaService.Contract.DTOs.MediaDTOs;
using MediaService.Contract.Infrastructure.Services;
using MediaService.Contract.Services.Media;
using MediaService.Presentation.Extensions;
using MongoDB.Bson;

namespace MediaService.Presentation.Endpoints.V1;
public static class MediaEndpoint
{
    public const string ApiName = "media";
    private const string BaseUrl = $"/media-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapMediaApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("", HandleUploadFilesAsync).WithMetadata(new RequireRolesAttribute("Patient", "SystemAdmin", "Moderator"));
        group.MapDelete("", HandleDeleteFilesAsync);

        return builder;
    }
    private static async Task<IResult> HandleUploadFilesAsync(ISender sender, IUserContext context,
        [FromForm] UploadImagesRequestDTO request)
    {
        var userId = context.UserId;
        var result = await sender.Send(new UploadFilesCommand{Images = request.Images, UploadedBy = userId ?? ""});
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
    
    private static async Task<IResult> HandleDeleteFilesAsync(ISender sender,
        [FromBody] List<string> imageIds)
    {
        var result = await sender.Send(new DeleteFilesCommand{ImageIds = imageIds});
        return result.IsSuccess ? Results.Ok(result.Value) : result.HandlerFailure();
    }
}