namespace NotificationService.Contract.EventBus.Events.Users;

public record UserInfoFcmTokenUpdatedIntegrationEvent : IntegrationEvent
{
    public string UserId { get; init; } = null!;
    public string? FcmToken { get; init; }
}