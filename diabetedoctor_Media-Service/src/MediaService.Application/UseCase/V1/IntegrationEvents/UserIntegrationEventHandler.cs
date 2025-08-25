using DnsClient.Internal;
using MediaService.Contract.Enumarations.User;
using MediaService.Contract.EventBus.Abstractions.Message;
using MediaService.Contract.EventBus.Events.UserIntegrationEvents;
using MediaService.Contract.Services.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediaService.Application.UseCase.V1.IntegrationEvents;

public class UserIntegrationEventHandler(ISender sender, ILogger<UserInfoCreatedProfileIntegrationEvent> logger)
    : IIntegrationEventHandler<UserInfoCreatedProfileIntegrationEvent>,
        IIntegrationEventHandler<UserInfoUpdatedProfileIntegrationEvent>
{
    public async Task Handle(UserInfoCreatedProfileIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification.UserId))
        {
            logger.LogWarning("Skipping because User Id is missing!");
            return;
        }
        await sender.Send(new CreateUserCommand(notification.UserId, notification.FullName, notification.Avatar, notification.Role), cancellationToken);
    }

    public async Task Handle(UserInfoUpdatedProfileIntegrationEvent notification, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(notification.UserId))
        {
            logger.LogWarning("Skipping because User Id is missing!");
            return;
        }
        await sender.Send(new UpdateUserCommand(notification.UserId, notification.FullName, notification.Avatar), cancellationToken);
    }
}