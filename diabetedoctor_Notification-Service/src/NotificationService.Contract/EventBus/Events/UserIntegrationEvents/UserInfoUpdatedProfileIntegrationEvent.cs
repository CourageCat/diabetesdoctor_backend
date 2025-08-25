using NotificationService.Contract.DTOs.ValueObjectDtos;

namespace NotificationService.Contract.EventBus.Events.UserIntegrationEvents;

public record UserInfoUpdatedProfileIntegrationEvent : IntegrationEvent
{
    public string? UserId { get; init; }
    public FullNameDto? FullName { get; init; }
    public string? Avatar { get; init; }
    public string? PhoneNumber { get; init; }
}