using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;

namespace AuthService.Api.Infrastructures.EventBus.Events;

public record UserInfoFcmTokenUpdatedIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public string? FcmToken { get; init; }
}