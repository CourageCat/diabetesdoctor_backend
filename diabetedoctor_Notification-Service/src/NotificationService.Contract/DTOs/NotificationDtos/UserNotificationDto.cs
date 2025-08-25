namespace NotificationService.Contract.DTOs.NotificationDtos;

public class UserNotificationDto
{
    public string? UserId { get; set; }
    public string? DeviceId { get; set; }
    public string? Type { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public string? Payload { get; set; }
}