using MediaService.Contract.Common.Constant.Event;
using MediaService.Contract.EventBus.Abstractions;
using MediaService.Contract.EventBus.Events.PostIntegrationEvents;
using MediaService.Contract.Services.Post;
using MediaService.Contract.Services.User;
namespace MediaService.Presentation.Endpoints.V1;
public static class UserEndpoint
{
    public const string ApiName = "users";
    private const string BaseUrl = $"/media-service/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapUserApiV1(this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.DisableAntiforgery();

        group.MapPost("", HandleCreateUserAsync);

        return builder;
    }

    private static async Task<IResult> HandleCreateUserAsync(IEventPublisher  eventPublisher)
    {
        //await eventPublisher.PublishAsync(MediaConstant.DeadTopic, new PostCreatedIntegrationEvent{PostId = "basubesu", Thumbnail = "123", Title = "dasd"}, 1);

        return Results.Ok();
    }

    private static IResult HandlerFailure(Result result) =>
        result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(),
            IValidationResult validationResult =>
                Results.BadRequest(
                    CreateProblemDetails(
                        "Validation Error", StatusCodes.Status400BadRequest,
                        result.Error,
                        validationResult.Errors)),
            _ =>
                Results.BadRequest(
                    CreateProblemDetails(
                        "Bab Request", StatusCodes.Status400BadRequest,
                        result.Error))
        };

    private static ProblemDetails CreateProblemDetails(
        string title,
        int status,
        Error error,
        Error[]? errors = null) =>
        new()
        {
            Title = title,
            Type = error.Code,
            Detail = (string)error.Message,
            Status = status,
            Extensions = { { nameof(errors), errors } }
        };
}