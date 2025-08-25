using ChatService.Contract.Enums;
using ConsultationService.Contract.EventBus.Abstractions.Message;
using ConsultationService.Contract.EventBus.Events.ConversationIntegrationEvents;
using ConsultationService.Contract.Services.Consultation.IntegrationCommands;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace ConsultationService.Infrastructure.EventBus.Kafka.EventHandlers;

public sealed class ConversationIntegrationEventHandler(ISender sender, ILogger<ConversationIntegrationEventHandler> logger) :
    IIntegrationEventHandler<ConversationCreatedIntegrationEvent>,
    IIntegrationEventHandler<ConversationLinkedIntegrationEvent>
{
    public async Task Handle(ConversationCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling conversation created event: {eventId}", notification.EventId);

        if (notification.ConversationType is ConversationTypeEnum.Group || string.IsNullOrWhiteSpace(notification.ConversationId) || string.IsNullOrWhiteSpace(notification.ConsultationId) || !ObjectId.TryParse(notification.ConsultationId, out var consultationId) )
        {
            logger.LogWarning("ConsultationId is missing. Skipping ...");
            return;
        }

        await sender.Send(new AttachConversationToConsultationCommand()
        {
            ConversationId = notification.ConversationId,
            ConsultationId = consultationId,
            IsOpened = notification.IsOpened
        }, cancellationToken);
    }

    public async Task Handle(ConversationLinkedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling conversation linked event: {eventId}", notification.EventId);

        if (string.IsNullOrWhiteSpace(notification.ConversationId) || string.IsNullOrWhiteSpace(notification.ConsultationId) || !ObjectId.TryParse(notification.ConsultationId, out var consultationId) )
        {
            logger.LogWarning("ConsultationId is missing. Skipping ...");
            return;
        }

        await sender.Send(new AttachConversationToConsultationCommand()
        {
            ConversationId = notification.ConversationId,
            ConsultationId = consultationId,
            IsOpened = notification.IsOpened
        }, cancellationToken);
    }
}