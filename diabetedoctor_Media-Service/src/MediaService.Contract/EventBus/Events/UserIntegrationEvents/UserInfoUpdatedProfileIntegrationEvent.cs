using MediaService.Contract.DTOs.UserDTOs;

namespace MediaService.Contract.EventBus.Events.UserIntegrationEvents;

public record UserInfoUpdatedProfileIntegrationEvent : IntegrationEvent
{
    public string UserId { get; init; } = null!;
    public FullNameDto? FullName { get; init; }
    public string? Avatar { get; init; }
}