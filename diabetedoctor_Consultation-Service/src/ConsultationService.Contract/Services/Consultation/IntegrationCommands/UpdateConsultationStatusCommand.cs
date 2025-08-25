using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.Enums;
using MongoDB.Bson;

namespace ConsultationService.Contract.Services.Consultation.IntegrationCommands;

public record UpdateConsultationStatusCommand : ICommand
{
    public ObjectId ConsultationId { get; init; }
    public bool IsDone { get; init; }
}