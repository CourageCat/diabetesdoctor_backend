using ChatService.Contract.EventBus.Events.ConsultationIntegrationEvents;
using ChatService.Contract.Services.Conversation.Commands.PersonalConversation;
using ChatService.Contract.Services.Conversation.IntegrationCommands;
using MongoDB.Bson;

namespace ChatService.Infrastructure.EventBus.Kafka.EventHandlers;

public sealed class ConsultationIntegrationEventHandler(
    ISender sender,
    ILogger<ConsultationIntegrationEventHandler> logger)
    : IIntegrationEventHandler<ConsultationBookedIntegrationEvent>,
    IIntegrationEventHandler<ConsultationStartedIntegrationEvent>,
    IIntegrationEventHandler<ConsultationEndedIntegrationEvent>
{
    public async Task Handle(ConsultationBookedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling consultation booked event to create private conversation for doctor - {doctorId} & patient - {patientId}",
            notification.DoctorId, notification.PatientId);
        if (string.IsNullOrWhiteSpace(notification.DoctorId) || string.IsNullOrWhiteSpace(notification.PatientId))
        {
            logger.LogWarning("Event missing doctorId or patientId. Skipping ...");
            return;
        }

        await sender.Send(new CreateConsultationConversationCommand
        {
            ConsultationId = notification.ConsultationId,
            PatientId = notification.PatientId,
            DoctorId = notification.DoctorId,
            IsOpened = notification.IsOpened
        }, cancellationToken);
    }

    public async Task Handle(ConsultationStartedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling opening private conversation: {conversationId}",
            notification.ConversationId);
        if (string.IsNullOrWhiteSpace(notification.ConversationId) ||
            !ObjectId.TryParse(notification.ConversationId, out var conversationId))
        {
            logger.LogWarning(
                "ConsultationBookedIntegrationEvent missing ConversationId or invalid Id format. Skipping user updating...");
            return;
        }

        await sender.Send(new ToggleGroupVisibilityCommand
        {
            ConversationId = conversationId,
            IsClosed = false
        }, cancellationToken);
    }

    public async Task Handle(ConsultationEndedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling closing private conversation: {conversationId}",
            notification.ConversationId);
        if (string.IsNullOrWhiteSpace(notification.ConversationId) ||
            !ObjectId.TryParse(notification.ConversationId, out var conversationId))
        {
            logger.LogWarning(
                "ConsultationBookedIntegrationEvent missing ConversationId or invalid Id format. Skipping user updating...");
            return;
        }

        await sender.Send(new ToggleGroupVisibilityCommand()
        {
            ConversationId = conversationId,
            IsClosed = true
        }, cancellationToken);
    }
}