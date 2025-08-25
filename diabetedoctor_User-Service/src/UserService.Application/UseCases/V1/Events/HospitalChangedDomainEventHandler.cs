using Microsoft.Extensions.Options;
using UserService.Application.Helper;
using UserService.Contract.EventBus.IntegrationEvents.Hospital;
using UserService.Contract.Settings;

namespace UserService.Application.UseCases.V1.Events;

public sealed class HospitalChangedDomainEventHandler(IRepositoryBase<OutboxEvent, Guid> outboxEventRepository, IOptions<KafkaSettings> kafkaSettings) : IDomainEventHandler<HospitalCreatedDomainEvent>
{
    public Task Handle(HospitalCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //Publish Event
        var integrationEvent = new HospitalCreatedIntegrationEvent
        {
            HospitalId = notification.HospitalProfile.Id,
            Name = notification.HospitalProfile.Name,
            Thumbnail = notification.HospitalProfile.Thumbnail.Url,
            Email = notification.HospitalProfile.Email,
            PhoneNumber = notification.HospitalProfile.PhoneNumber,
        };
        var outboxEvent =
            OutboxEventExtension.ToOutboxEvent(kafkaSettings.Value.HospitalTopic,
                integrationEvent);
        outboxEventRepository.Add(outboxEvent);
        return Task.CompletedTask;
    }
}