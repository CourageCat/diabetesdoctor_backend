using UserService.Contract.EventBus.IntegrationEvents.Consultation;
using UserService.Contract.Services.Doctors.Commands;
using UserService.Contract.Services.Patients.Commands;

namespace UserService.Infrastructure.EventBus.Kafka.EventHandlers;

public class ConsultationIntegrationEventHandler
    (ISender sender)
    : IIntegrationEventHandler<ConsultationBookedIntegrationEvent>,
        IIntegrationEventHandler<ConsultationCancelledIntegrationEvent>,
        IIntegrationEventHandler<ConsultationEndedIntegrationEvent>
{
    public async Task Handle(ConsultationBookedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(new UseConsultationSessionCommand{UserId = @event.PatientId}, cancellationToken);
    }

    public async Task Handle(ConsultationCancelledIntegrationEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(new RefundConsultationSessionCommand{UserId = @event.UserId, UserPackageId = @event.UserPackageId}, cancellationToken);
    }

    public async Task Handle(ConsultationEndedIntegrationEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(new ReceiveMoneyFromConsultationCommand{UserId = @event.UserId, ConsultationFee = @event.ConsultationFee}, cancellationToken);
    }
}