namespace UserService.Contract.EventBus.IntegrationEvents.UserInfo;

public record UserInfoCreatedProfileIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public FullNameDto FullName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!;
    public string? Email { get; init; } = null!;
    public Guid? HospitalId  { get; init; }
    public int Role  { get; init; }
}