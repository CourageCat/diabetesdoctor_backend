namespace NotificationService.Contract.Infrastructure;

public interface IClaimsService
{
    public string GetCurrentUserId { get; }
    public string GetCurrentRole { get; }
}