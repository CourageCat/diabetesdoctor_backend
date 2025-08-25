using NotificationService.Contract.Enums;
using NotificationService.Contract.EventBus.Events.Notifications;
using NotificationService.Contract.Services.Notification;

namespace NotificationService.Application.UseCases.V1.IntegrationCommands;

public class PostIntegrationEventHandler (ISender sender, ILogger<PostIntegrationEventHandler> logger) :
    IIntegrationEventHandler<PostCreatedIntegrationEvent>
{
    public async Task Handle(PostCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling post created event: {postId}", notification.PostId);
        if (string.IsNullOrWhiteSpace(notification.PostId) || !ObjectId.TryParse(notification.PostId, out _))
        {
            logger.LogWarning("PostCreatedIntegrationEvent missing Id. Skipping chat creation...");
            return;
        }
        
        await sender.Send(new CreatePostNotificationCommand(notification.PostId, notification.Title, notification.Thumbnail, NotificationTypeEnum.Post), cancellationToken);
    }
}