using NotificationService.Contract.EventBus.Events.Users;
using NotificationService.Contract.Services.User;

namespace NotificationService.Infrastructure.EventBus.Kafka.EventHandlers;

public class UserIntegrationEventHandler(ISender sender, ILogger<UserIntegrationEventHandler> logger) :
    IIntegrationEventHandler<UserInfoCreatedProfileIntegrationEvent>, 
    IIntegrationEventHandler<UserInfoUpdatedProfileIntegrationEvent>,
    IIntegrationEventHandler<UserInfoFcmTokenUpdatedIntegrationEvent>
    {
        public async Task Handle(UserInfoCreatedProfileIntegrationEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling user created event: {userId}", notification.UserId);
            
            if (string.IsNullOrWhiteSpace(notification.UserId))
            {
                logger.LogWarning("UserCreatedIntegrationEvent missing Id. Skipping user creation...");
                return;
            }
            
            await sender.Send(new CreateUserCommand
            {
                UserId = notification.UserId,
                FullName = notification.FullName,
                Avatar = notification.Avatar
            }, cancellationToken);
        }

        public async Task Handle(UserInfoUpdatedProfileIntegrationEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling user updated event: {userId}", notification.UserId);
        
            if (string.IsNullOrWhiteSpace(notification.UserId))
            {
                logger.LogWarning("UserUpdatedIntegrationEvent missing Id. Skipping user updating...");
                return;
            }
            
            await sender.Send(new UpdateUserCommand
            {
                UserId = notification.UserId,
                FullName = notification.FullName,
                Avatar = notification.Avatar
            }, cancellationToken);
        }

        public async Task Handle(UserInfoFcmTokenUpdatedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling user FcmToken updated event: {userId}", notification.UserId);
        
            if (string.IsNullOrWhiteSpace(notification.UserId))
            {
                logger.LogWarning("UserInfoFcmTokenUpdatedIntegrationEvent missing Id. Skipping user updating...");
                return;
            }
            
            await sender.Send(new UpdateUserFcmTokenCommand
            {
                UserId = notification.UserId,
                FcmToken = notification.FcmToken
            }, cancellationToken);
        }
    }
