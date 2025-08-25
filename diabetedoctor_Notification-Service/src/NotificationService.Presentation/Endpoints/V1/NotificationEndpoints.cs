using Result = NotificationService.Contract.Abstractions.Shared.Result;
using NotificationService.Contract.Abstractions.Shared;
using NotificationService.Contract.DTOs.NotificationDtos;
using NotificationService.Contract.EventBus.Events.Notifications;
using NotificationService.Contract.EventBus.Abstractions;
using NotificationService.Contract.Infrastructure;
using Aspire.Confluent.Kafka;
using NotificationService.Contract.Common.Constants;
using NotificationService.Contract.Services;
using NotificationService.Contract.Services.Notification;


namespace NotificationService.Presentation.Endpoints.V1;

public static class NotificationEndpoints
{
    public const string ApiName = "notifications";
    private const string BaseUrl = $"/api/v{{version:apiVersion}}/{ApiName}";

    public static IVersionedEndpointRouteBuilder MapNotificationsApiV1(
        this IVersionedEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup(BaseUrl).HasApiVersion(1);
        group.MapPost("/publish/kafka", PublishUserNotificationAsync);
        group.MapGet("user", GetUserNotificationsAsync).RequireAuthorization();
        group.MapPost("", HandleCreateNotificationAsync).RequireAuthorization();
        group.MapDelete("{notificationId}", HandleDeleteNotificationAsync);
        group.MapPatch("", MarkNotificationReadAsync);
        return builder;
    }


    private static async Task<IResult> PublishUserNotificationAsync(
    [FromBody] PostCreatedIntegrationEvent eventData,
    [FromServices] IEventPublisher eventPublisher)
    {
        await eventPublisher.PublishAsync(KafkaTopicConstraints.DeadTopic, new PostCreatedIntegrationEvent { PostId = "basubesu", Thumbnail = "123", Title = "dasd" }, 1);

        return Results.Ok();
    }
    
    private static async Task<IResult> GetUserNotificationsAsync(ISender sender, IClaimsService claimsService, [AsParameters] QueryFilter filter)
    {
        var userId = claimsService.GetCurrentUserId;
        var result = await sender.Send(new GetNotificationsByUserIdQuery() { UserId = userId, Filter = filter });
        return result.IsFailure ? HandlerFailure(result) : Results.Ok(result);
    }
    private static async Task<IResult> HandleCreateNotificationAsync(ISender sender, IClaimsService claimsService,
        [FromBody] CreateNotificationRequestDto request)
    {
        var userId = claimsService.GetCurrentUserId;
        var createCommand = new CreateNotificationCommand(request.Title, request.Body, request.Thumbnail, request.NotificationTypeEnum, request.UserIds, userId);
        var result = await sender.Send(createCommand);
        if (result.IsFailure)
            return HandlerFailure(result);

        return Results.Ok(result);
    }

    private static async Task<IResult> HandleDeleteNotificationAsync(ISender sender, IClaimsService claimsService, string notificationId)
    {
        var userId = claimsService.GetCurrentUserId;
        var result = await sender.Send(new DeleteNotificationCommand(userId, notificationId));
        return result.IsFailure ? HandlerFailure(result) : Results.Ok(result);
    }

    private static async Task<IResult> MarkNotificationReadAsync(ISender sender, IClaimsService claimsService, List<string> notificationIds)
    {
        var userId = claimsService.GetCurrentUserId;
        var result = await sender.Send(new MarkReadNotificationCommand(userId, notificationIds));
        return result.IsFailure ? HandlerFailure(result) : Results.Ok(result);
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
            Detail = error.Message,
            Status = status,
            Extensions = { { nameof(errors), errors } }
        };
}