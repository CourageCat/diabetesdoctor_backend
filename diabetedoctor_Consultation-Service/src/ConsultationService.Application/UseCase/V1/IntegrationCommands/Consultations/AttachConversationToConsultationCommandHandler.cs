using System.Text.Json;
using ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;
using ConsultationService.Contract.Exceptions.BusinessExceptions;
using ConsultationService.Contract.Services.Consultation.IntegrationCommands;
using ConsultationService.Contract.Settings;
using Microsoft.Extensions.Options;

namespace ConsultationService.Application.UseCase.V1.IntegrationCommands.Consultations;

public sealed class AttachConversationToConsultationCommandHandler(
    IUnitOfWork unitOfWork,
    IOptions<KafkaSettings> kafkaSettings,
    IConsultationRepository consultationRepository,
    IOutboxScheduleEventRepository outboxScheduleEventRepository)
    : ICommandHandler<AttachConversationToConsultationCommand>
{
    public async Task<Result> Handle(AttachConversationToConsultationCommand request, CancellationToken cancellationToken)
    {
        var consultation = await consultationRepository.FindSingleAsync(c => c.Id == request.ConsultationId, cancellationToken: cancellationToken);
        if (consultation is null)
        {
            throw new ConsultationExceptions.ConsultationNotFoundException();
        }

        var conversationId = ConversationId.Of(request.ConversationId);
        consultation.LinkConversation(conversationId);
        await consultationRepository.ReplaceOneAsync(unitOfWork.ClientSession, consultation, cancellationToken: cancellationToken);
        var scheduleEvent = MapToIntegrationScheduleEvent(consultation, consultation.StartTime, consultation.EndTime, request.IsOpened);
        await outboxScheduleEventRepository.CreateManyAsync(unitOfWork.ClientSession, scheduleEvent, cancellationToken: cancellationToken);
        return Result.Success();
    }
    
    private List<OutboxScheduleEvent> MapToIntegrationScheduleEvent(Consultation consultation, DateTime startTime, DateTime endTime, bool isOpened)
    {
        var scheduleEvents = new List<OutboxScheduleEvent>();
        if (!isOpened)
        {
            var consultationStartedEvent = new ConsultationStartedIntegrationEvent
            {
                ConversationId = consultation.ConversationId!.ToString(),
                ConsultationId = consultation.Id.ToString(),
            };
            var consultationStartedEventType = consultationStartedEvent.GetType();
            scheduleEvents.Add(OutboxScheduleEvent.Create(
                id: ObjectId.GenerateNewId(), 
                topic: kafkaSettings.Value.ConsultationTopic,
                eventTypeName: consultationStartedEvent.GetType().Name,
                message: JsonSerializer.Serialize(consultationStartedEvent, consultationStartedEventType),
                visibleAt: startTime));
        }
        
        var consultationEndedEvent = new ConsultationEndedIntegrationEvent
        {
            ConversationId = consultation.ConversationId!.ToString(),
            ConsultationId = consultation.Id.ToString(),
            UserId = consultation.PatientId.ToString(),
            ConsultationFee = consultation.UserPackage!.Price
        };
        var consultationEndedEventType = consultationEndedEvent.GetType();
        scheduleEvents.Add(OutboxScheduleEvent.Create(
            id: ObjectId.GenerateNewId(),
            topic: kafkaSettings.Value.ConsultationTopic,
            eventTypeName: consultationEndedEvent.GetType().Name,
            message: JsonSerializer.Serialize(consultationEndedEvent, consultationEndedEventType),
            visibleAt: endTime));
        
        return scheduleEvents;
    }
}