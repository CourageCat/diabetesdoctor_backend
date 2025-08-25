using MongoDB.Bson;
using NotificationService.Contract.EventBus.Events.Conversations;
using NotificationService.Contract.Services.Conversation;
using NotificationService.Contract.Services.Conversation.Commands;

namespace NotificationService.Infrastructure.EventBus.Kafka.EventHandlers;

public class ConversationIntegrationEventHandler(ISender sender, ILogger<ConversationIntegrationEventHandler> logger) : 
    IIntegrationEventHandler<ConversationCreatedIntegrationEvent>,
    IIntegrationEventHandler<GroupMembersAddedIntegrationEvent>, 
    IIntegrationEventHandler<ConversationUpdatedIntegrationEvent>,
    IIntegrationEventHandler<GroupMemberRemovedIntegrationEvent>,
    IIntegrationEventHandler<ConversationDeletedIntegrationEvent>

{
    public async Task Handle(ConversationCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling group created event: {groupId}", notification.ConversationId);
        
        if (string.IsNullOrWhiteSpace(notification.ConversationId) || !ObjectId.TryParse(notification.ConversationId, out _))
        {
            logger.LogWarning("GroupCreatedIntegrationEvent missing GroupId. Skipping group creation...");
            return;
        }

        await sender.Send(new CreateConversationCommand
        {
            ConversationId = notification.ConversationId,
            ConversationName = notification.ConversationName,
            Avatar = notification.Avatar,
            Members = notification.Members
        }, cancellationToken);
    }

    public async Task Handle(GroupMembersAddedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling group created event: {groupId}", notification.ConversationId);
        
        if (string.IsNullOrWhiteSpace(notification.ConversationId) || !ObjectId.TryParse(notification.ConversationId, out _))
        {
            logger.LogWarning("GroupMembersAddedIntegrationEvent GroupId Id. Skipping members addition...");
            return;
        }
        
        await sender.Send(new AddMemberToGroupCommand
        {
            ConversationId = notification.ConversationId,
            Members = notification.Members
        }, cancellationToken);
    }

    public async Task Handle(ConversationUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling group updated event: {groupId}", notification.ConversationId);

        if (string.IsNullOrWhiteSpace(notification.ConversationId) || !ObjectId.TryParse(notification.ConversationId, out _))
        {
            logger.LogWarning("GroupMembersUpdatedIntegrationEvent GroupId Id. Skipping members updation...");
            return;
        }

        await sender.Send(new UpdateConversationCommand
        {
            ConversationId = notification.ConversationId,
            Name = notification.ConversationName,
            Avatar = notification.Avatar,
        }, cancellationToken);
    }

    public async Task Handle(GroupMemberRemovedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling group member remove event: {groupId}", notification.ConversationId);

        if (string.IsNullOrWhiteSpace(notification.ConversationId) || !ObjectId.TryParse(notification.ConversationId, out _))
        {
            logger.LogWarning("GroupMembersRemovedIntegrationEvent GroupId Id. Skipping remove member...");
            return;
        }
        
        await sender.Send(new RemoveConversationMemberCommand
        {
            ConversationId = notification.ConversationId,
            MemberId = notification.MemberId
        }, cancellationToken);
    }

    public async Task Handle(ConversationDeletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling group member remove event: {groupId}", notification.ConversationId);

        if (string.IsNullOrWhiteSpace(notification.ConversationId) || !ObjectId.TryParse(notification.ConversationId, out _))
        {
            logger.LogWarning("GroupMembersRemovedIntegrationEvent GroupId Id. Skipping remove member...");
            return;
        }

        await sender.Send(new DeleteConversationCommand
        {
            ConversationId = notification.ConversationId
        }, cancellationToken);
    }
}