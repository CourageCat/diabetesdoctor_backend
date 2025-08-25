using ConsultationService.Application.Helpers;
using ConsultationService.Contract.EventBus.Events.ConsultationIntegrationEvents;
using ConsultationService.Contract.Services.Consultation.Commands;
using ConsultationService.Contract.Settings;
using ConsultationService.Domain.Enums;
using Microsoft.Extensions.Options;

namespace ConsultationService.Application.UseCase.V1.Commands.Consultations;

public sealed class CancelConsultationCommandHandler(
    IUnitOfWork unitOfWork,
    IOptions<KafkaSettings> kafkaSettings,
    IConsultationRepository consultationRepository,
    IConsultationTemplateRepository consultationTemplateRepository,
    IOutboxEventRepository outboxEventRepository,
    IOutboxScheduleEventRepository outboxScheduleEventRepository)
    : ICommandHandler<CancelConsultationCommand, Response>
{
    public async Task<Result<Response>> Handle(CancelConsultationCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.Of(request.UserId);
        var consultation = await consultationRepository.FindSingleAsync(
            c => c.Id == request.ConsultationId
            && (c.PatientId == userId || c.DoctorId == userId
            && c.Status == ConsultationStatus.Booked),
            cancellationToken: cancellationToken);
        if (consultation is null)
        {
            return Result.Failure<Response>(ConsultationErrors.NotFound);
        }
        
        var integrationEvent = MapToCancelledIntegrationEvent(userId, consultation.UserPackage!.ToString());
        var cancel = request.Role == nameof(Role.Doctor) ? consultation.Decline(request.Reason) : consultation.Cancel(request.Reason);
        if (cancel.IsFailure)
        {
            return Result.Failure<Response>(cancel.Error);    
        }

        var template = await consultationTemplateRepository.FindSingleAsync(
            ct => ct.Id == consultation.ConsultationTemplateId,
            cancellationToken: cancellationToken);
        if (template is null)
        {
            return Result.Failure<Response>(ConsultationTemplateErrors.NotFound);
        }
        template.Available();
        
        try
        {
            await unitOfWork.StartTransactionAsync(cancellationToken);
            await consultationRepository.ReplaceOneAsync(unitOfWork.ClientSession, consultation, cancellationToken);
            await consultationTemplateRepository.ReplaceOneAsync(unitOfWork.ClientSession, template, cancellationToken);
            
            var filters = Builders<OutboxScheduleEvent>.Filter.Regex(
                schedule => schedule.Message,
                new BsonRegularExpression(consultation.Id.ToString()));
            await outboxScheduleEventRepository.DeleteManyAsync(unitOfWork.ClientSession, filters, cancellationToken);
            
            var outboxEvent = OutboxEventExtension.ToOutboxEvent(kafkaSettings.Value.ConsultationTopic, integrationEvent);
            await outboxEventRepository.CreateAsync(unitOfWork.ClientSession, outboxEvent, cancellationToken);
            
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await unitOfWork.AbortTransactionAsync(cancellationToken);
            throw;
        }
        return Result.Success(new Response(
            ConsultationMessage.CancelConsultationSuccessfully.GetMessage().Code, 
            ConsultationMessage.CancelConsultationSuccessfully.GetMessage().Message));
    }

    private static ConsultationCancelledIntegrationEvent MapToCancelledIntegrationEvent(UserId userId, string packageId, string? reason = null)
    {
        return new ConsultationCancelledIntegrationEvent
        {
            UserId = userId.ToString(),
            UserPackageId = packageId,
            Reason = reason,
        };
    }
}