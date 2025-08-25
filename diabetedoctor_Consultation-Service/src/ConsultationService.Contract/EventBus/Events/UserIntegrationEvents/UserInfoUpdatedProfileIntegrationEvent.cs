using ConsultationService.Contract.DTOs.ValueObjectDtos;
using ConsultationService.Contract.EventBus.Abstractions.Message;

namespace ConsultationService.Contract.EventBus.Events.UserIntegrationEvents;

public record UserInfoUpdatedProfileIntegrationEvent : IntegrationEvent
{
    public string? UserId { get; init; }
    public FullNameDto? FullName { get; init; }
    public string? Avatar { get; init; }
    public string? PhoneNumber { get; init; }
}