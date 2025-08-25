using AuthService.Api.Infrastructures.Abstractions.EventsBus.Message;

namespace AuthService.Api.Infrastructures.EventBus.Events;

public record UserInfoCreatedProfileIntegrationEvent : IntegrationEvent
{
    public Guid UserId { get; init; }
    public FullNameDto FullName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!;
    public string? Email { get; init; } = null!;
    public int Role  { get; init; }
}