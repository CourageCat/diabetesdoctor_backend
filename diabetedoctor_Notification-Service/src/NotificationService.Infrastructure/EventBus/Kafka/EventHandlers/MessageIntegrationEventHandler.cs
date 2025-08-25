using NotificationService.Contract.EventBus.Events.Notifications;
using NotificationService.Contract.Services.Notification.Commands;

namespace NotificationService.Infrastructure.EventBus.Kafka.EventHandlers;

public sealed class MessageIntegrationEventHandler(
    ISender sender, 
    ILogger<MessageIntegrationEventHandler> logger)
    : IIntegrationEventHandler<MessageCreatedIntegrationEvent>
{
    public async Task Handle(MessageCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling message created event: {messageId}", notification.MessageId);
        if (string.IsNullOrWhiteSpace(notification.ConversationId) || string.IsNullOrWhiteSpace(notification.SenderId) || string.IsNullOrWhiteSpace(notification.MessageId))
        {
            logger.LogWarning("MessageCreatedIntegrationEvent missing Id. Skipping mesage creation...");
            return;
        }

        await sender.Send(new PushChatNotificationCommand
        {
            SenderId = notification.SenderId,
            ConversationId = notification.ConversationId,
            ConversationType = notification.ConversationType,
            MessageId = notification.MessageId,
            MessageContent = notification.MessageContent,
            MessageType = notification.MessageType,
            FileAttachment = notification.FileAttachment,
            CreatedDate = notification.CreatedDate
        }, cancellationToken);
    }
}