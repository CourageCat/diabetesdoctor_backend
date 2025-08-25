using ConsultationService.Contract.EventBus.Abstractions.Message;
using ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;
using ConsultationService.Contract.Services.Consultation.IntegrationCommands;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace ConsultationService.Infrastructure.EventBus.Kafka.EventHandlers;

public sealed class ConsultationIntegrationEventHandler(
    ISender sender, ILogger<ConsultationIntegrationEventHandler> logger)
    : 
        IIntegrationEventHandler<ConsultationStartedIntegrationEvent>, 
        IIntegrationEventHandler<ConsultationEndedIntegrationEvent>
{
    public async Task Handle(ConsultationStartedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling consultation started event: {eventId}", notification.EventId);

        if (string.IsNullOrWhiteSpace(notification.ConsultationId) || !ObjectId.TryParse(notification.ConsultationId, out var consultationId) )
        {
            logger.LogWarning("ConsultationId is missing. Skipping ...");
            return;
        }

        await sender.Send(new UpdateConsultationStatusCommand
        {
            ConsultationId = consultationId,
            IsDone = false
        }, cancellationToken);
    }

    public async Task Handle(ConsultationEndedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling consultation ended event: {eventId}", notification.EventId);

        if (string.IsNullOrWhiteSpace(notification.ConsultationId) || !ObjectId.TryParse(notification.ConsultationId, out var consultationId) )
        {
            logger.LogWarning("ConsultationId is missing. Skipping ...");
            return;
        }
        await sender.Send(new UpdateConsultationStatusCommand
        {
            ConsultationId = consultationId,
            IsDone = true
        }, cancellationToken);
    }
}