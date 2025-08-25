using UserService.Contract.Services.Users;

namespace UserService.Infrastructure.EventBus.Kafka.EventHandlers;

public class WhenPatientCreatedIntegrationEventHandler
    (ISender sender)
    : IIntegrationEventHandler<PatientCreatedAccountIntegrationEvent>
{
    public async Task Handle(PatientCreatedAccountIntegrationEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(new CreateUserCommand(@event.UserId), cancellationToken);
    }
}
