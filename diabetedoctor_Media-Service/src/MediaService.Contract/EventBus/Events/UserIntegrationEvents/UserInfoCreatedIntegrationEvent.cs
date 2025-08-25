using MediaService.Contract.DTOs.UserDTOs;

namespace MediaService.Contract.EventBus.Events.UserIntegrationEvents;

public record UserInfoCreatedProfileIntegrationEvent : IntegrationEvent
{
    public string UserId { get; init; } = null!;
    public FullNameDto FullName { get; init; } = null!;
    public string Avatar { get; init; } = null!;
    public string? PhoneNumber { get; init; } = null!;
    public string? Email { get; init; } = null!;
    public string? HospitalId  { get; init; }
    public int Role  { get; init; }
}