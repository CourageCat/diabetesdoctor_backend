using ConsultationService.Contract.Abstractions.Message;
using ConsultationService.Contract.Abstractions.Shared;
using MongoDB.Bson;

namespace ConsultationService.Contract.Services.Consultation.Commands;

public record CancelConsultationCommand : ICommand<Response>
{
    public string UserId { get; init; } = null!;
    public string Role { get; init; } = null!;
    public string? Reason { get; init; }
    public ObjectId ConsultationId { get; init; }
}