using NotificationService.Contract.DTOs.ValueObjectDtos;

namespace NotificationService.Contract.EventBus.Events.Users;

public record UserInfoCreatedProfileIntegrationEvent : IntegrationEvent
{
    public string UserId { get; init; } = null!;
    public FullNameDto FullName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
}