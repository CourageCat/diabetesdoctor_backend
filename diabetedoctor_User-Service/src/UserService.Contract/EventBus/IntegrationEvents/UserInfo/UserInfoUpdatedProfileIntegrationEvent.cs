namespace UserService.Contract.EventBus.IntegrationEvents.UserInfo;

public record UserInfoUpdatedProfileIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public FullNameDto? FullName { get; init; }
    public string? Avatar { get; init; }
}