using ChatService.Contract.EventBus.Events.MessageIntegrationEvents;
using ChatService.Contract.Services.Conversation.IntegrationCommands;
using MongoDB.Bson;

namespace ChatService.Infrastructure.EventBus.Kafka.EventHandlers;

public sealed class MessageIntegrationEventHandler(ISender sender, ILogger<MessageIntegrationEventHandler> logger) : 
    IIntegrationEventHandler<MessageCreatedIntegrationEvent>
{
    public async Task Handle(MessageCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling message created event: {messageId}", notification.MessageId);

        if (!ObjectId.TryParse(notification.MessageId, out var messageId) ||
            !ObjectId.TryParse(notification.ConversationId, out var conversationId))
        {
            logger.LogError("Failed to parse MessageId ({MessageId}) or ConversationId ({ConversationId})",
                notification.MessageId,
                notification.ConversationId);
            return;
        }
        await sender.Send(new UpdateLastMessageInConversationCommand
        {
            ConversationId = conversationId,
            SenderId = notification.SenderId,
            MessageId = messageId,
            MessageContent = notification.MessageContent,
            MessageType = notification.MessageType,
            FileAttachmentDto = notification.FileAttachment,
            CreatedDate = notification.CreatedDate,
        }, cancellationToken);
    }
}