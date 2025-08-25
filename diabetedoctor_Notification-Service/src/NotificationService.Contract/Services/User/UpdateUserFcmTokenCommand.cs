namespace NotificationService.Contract.Services.User;

public record UpdateUserFcmTokenCommand : ICommand
{
    public string UserId { get; init; } = null!;
    public string? FcmToken { get; init; }
}