using Microsoft.Extensions.Options;
using UserService.Application.Helper;
using UserService.Application.Mapping;
using UserService.Contract.EventBus.IntegrationEvents.UserInfo;
using UserService.Contract.Settings;

namespace UserService.Application.UseCases.V1.Events;

public sealed class UserInfoChangedDomainEventHandler(
    IRepositoryBase<OutboxEvent, Guid> outboxEventRepository,
    IOptions<KafkaSettings> kafkaSettings,
    IUnitOfWork unitOfWork)
    : IDomainEventHandler<UserInfoCreatedDomainEvent>, IDomainEventHandler<UserInfoUpdatedDomainEvent>
{
    public Task Handle(UserInfoCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //Publish Event
        var integrationEvent = new UserInfoCreatedProfileIntegrationEvent
        {
            UserId = notification.UserInfo.Id,
            FullName = Mapper.MapFullNameDto(notification.UserInfo.FullName),
            Avatar = notification.UserInfo.Avatar.Url,
            PhoneNumber = notification.UserInfo.PhoneNumber,
            Email = notification.UserInfo.Email,
            HospitalId = notification.HospitalId,
            Role = (int)notification.Role,
        };
        var outboxEvent =
            OutboxEventExtension.ToOutboxEvent(kafkaSettings.Value.UserTopic,
                integrationEvent);
        outboxEventRepository.Add(outboxEvent);
        return Task.CompletedTask;
    }

    public Task Handle(UserInfoUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //Publish Event
        var integrationEvent = new UserInfoUpdatedProfileIntegrationEvent
        {
            UserId = notification.UserId,
            FullName = notification.FullName != null ? Mapper.MapFullNameDto(notification.FullName!) : null,
            Avatar = notification.Avatar
        };
        var outboxEvent =
            OutboxEventExtension.ToOutboxEvent(kafkaSettings.Value.UserTopic,
                integrationEvent);
        outboxEventRepository.Add(outboxEvent);
        return Task.CompletedTask;
    }
}