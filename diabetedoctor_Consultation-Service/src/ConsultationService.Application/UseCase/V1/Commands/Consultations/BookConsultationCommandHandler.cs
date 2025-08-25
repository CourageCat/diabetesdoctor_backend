using System.Text.Json;
using ConsultationService.Application.Helpers;
using ConsultationService.Application.Protos.Client.UserPackage;
using ConsultationService.Contract.EventBus.Abstractions.Message;
using ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;
using ConsultationService.Contract.Helpers;
using ConsultationService.Contract.Services.Consultation;
using ConsultationService.Contract.Services.Consultation.Commands;
using ConsultationService.Contract.Settings;
using ConsultationService.Domain.Enums;
using Microsoft.Extensions.Options;
using UserPackage = ConsultationService.Application.Protos.Client.UserPackage.UserPackage;
using UserPackageValueObject = ConsultationService.Domain.ValueObjects.UserPackage;

namespace ConsultationService.Application.UseCase.V1.Commands.Consultations;

public sealed class BookConsultationCommandHandler(
    IUnitOfWork unitOfWork,
    IOptions<KafkaSettings> kafkaSettings,
    UserPackage.UserPackageClient userPackageClient,
    IConsultationRepository consultationRepository,
    IConsultationTemplateRepository templateRepository,
    IOutboxEventRepository outboxEventRepository) 
    : ICommandHandler<BookConsultationCommand, Response>
{
    public async Task<Result<Response>> Handle(BookConsultationCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(request.UserId);
        _ = ObjectId.TryParse(request.TemplateId, out var templateId);
        var template = await templateRepository.FindSingleAsync(
            c => c.Id == templateId
                && c.IsDeleted == false,
            cancellationToken: cancellationToken);
        
        if (template is null)
        {
            return Result.Failure<Response>(ConsultationErrors.NotFound);
        }

        if (template.Status is ConsultationTemplateStatus.Booked)
        {
            return Result.Failure<Response>(ConsultationTemplateErrors.TemplateIsBooked);
        }

        // var checkSessionRemainingAsync = await userPackageClient.CheckSessionRemainingAsync(new CheckSessionRemainingRequest {UserId = userId.ToString()}, cancellationToken: cancellationToken);
        // if (!checkSessionRemainingAsync.IsSuccess)
        // {
        //     return Result.Failure<Response>(ConsultationErrors.ConsultationSessionsNotEnough);
        // }
        //
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            var isOpened = (template.Date + TimeSpan.Parse(template.StartTime)) - CurrentTimeService.GetVietNamCurrentTime() <= TimeSpan.FromMinutes(2);
            
            // var consultation = MapToConsultation(userId, template, isOpened, checkSessionRemainingAsync);
            var consultation = MapToConsultation(userId, template, isOpened);
            
            if (consultation.IsFailure && consultation is IValidationResult validationResult)
            {
                return ValidationResult<Response>.WithErrors(validationResult.Errors.AsEnumerable().ToArray());
            }
            await consultationRepository.CreateAsync(unitOfWork.ClientSession, consultation.Value, cancellationToken);
            
            template.Booked();
            await templateRepository.ReplaceOneAsync(unitOfWork.ClientSession, template, cancellationToken: cancellationToken);
            
            var integrationEvent = MapToIntegrationEvent(consultation.Value.Id, template.DoctorId, userId, isOpened);
            var @event = OutboxEventExtension.ToOutboxEvent(kafkaSettings.Value.ConsultationTopic, integrationEvent);
            await outboxEventRepository.CreateAsync(unitOfWork.ClientSession, @event, cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
            
            // fix flow consume event to add conversationId after conversation created (type = consultation, other skipping)
            // var scheduleEndEvent = MapToIntegrationScheduleEvent(consultation.Value.StartTime, consultation.Value.EndTime, isOpened);
            // await outboxScheduleEventRepository.CreateManyAsync(unitOfWork.ClientSession, scheduleEndEvent, cancellationToken);
        }
        catch (Exception e)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        
        return Result.Success(new Response(
            ConsultationMessage.BookConsultationSuccessfully.GetMessage().Code,
            ConsultationMessage.BookConsultationSuccessfully.GetMessage().Message));
    }

    // private static Result<Consultation> MapToConsultation(UserId userId, ConsultationTemplate template, bool isOpened, CheckSessionRemainingResponse response)
    // {
    //     var id = ObjectId.GenerateNewId();
    //     var startTime = TimeSpan.Parse(template.StartTime);
    //     var endTime = TimeSpan.Parse(template.EndTime);
    //     var package = UserPackageValueObject.Create(response.UserPackageId, response.Price);
    //     return Consultation.Create(
    //         id: id,
    //         date: template.Date,
    //         startTime: startTime,
    //         endTime: endTime,
    //         doctorId: template.DoctorId,
    //         patientId: userId,
    //         templateId: template.Id,
    //         isOpened: isOpened,
    //         userPackage: package);
    // }
    
    private static Result<Consultation> MapToConsultation(UserId userId, ConsultationTemplate template, bool isOpened)
    {
        var id = ObjectId.GenerateNewId();
        var startTime = TimeSpan.Parse(template.StartTime);
        var endTime = TimeSpan.Parse(template.EndTime);
        var package = UserPackageValueObject.Create("abc", 50000);
        return Consultation.Create(
            id: id,
            date: template.Date,
            startTime: startTime,
            endTime: endTime,
            doctorId: template.DoctorId,
            patientId: userId,
            templateId: template.Id,
            isOpened: isOpened,
            userPackage: package);
    }

    private static ConsultationBookedIntegrationEvent MapToIntegrationEvent(ObjectId consultationId, UserId doctorId, UserId userId, bool isOpened)
    {
        return new ConsultationBookedIntegrationEvent()
        {
            ConsultationId = consultationId.ToString(),
            DoctorId = doctorId.ToString(),
            PatientId = userId.ToString(),
            IsOpened = isOpened
        };
    }
    
    // private List<OutboxScheduleEvent> MapToIntegrationScheduleEvent(DateTime startTime, DateTime endTime, bool isOpened)
    // {
    //     var scheduleEvents = new List<OutboxScheduleEvent>();
    //     if (!isOpened)
    //     {
    //         var consultationStartedEvent = new ConsultationStartedIntegrationEvent { ConversationId = conversationId.ToString() };
    //         var consultationStartedEventType = consultationStartedEvent.GetType();
    //         scheduleEvents.Add(OutboxScheduleEvent.Create(
    //             id: ObjectId.GenerateNewId(), 
    //             topic: kafkaSettings.Value.ConsultationTopic,
    //             eventTypeName: consultationStartedEvent.GetType().Name,
    //             message: JsonSerializer.Serialize(consultationStartedEvent, consultationStartedEventType),
    //             visibleAt: startTime));
    //     }
    //     
    //     var consultationEndedEvent = new ConsultationEndedIntegrationEvent{ConversationId = conversationId.ToString()};
    //     var consultationEndedEventType = consultationEndedEvent.GetType();
    //     scheduleEvents.Add(OutboxScheduleEvent.Create(
    //         id: ObjectId.GenerateNewId(),
    //         topic: kafkaSettings.Value.ConsultationTopic,
    //         eventTypeName: consultationEndedEvent.GetType().Name,
    //         message: JsonSerializer.Serialize(consultationEndedEvent, consultationEndedEventType),
    //         visibleAt: endTime));
    //     
    //     return scheduleEvents;
    // }
}