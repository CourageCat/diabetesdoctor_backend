using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;

namespace AuthService.Api.Infrastructures.EventBus.Events;

public record UserInfoUpdatedProfileIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public FullNameDto? FullName { get; init; }
    public string? Avatar { get; init; }
}