using NotificationService.Contract.Enums;

namespace NotificationService.Contract.DTOs.NotificationDtos;

public class CreateNotificationRequestDto
{
    public string Title { get; init; }
    public string Body { get; init; }
    public string Thumbnail { get; init; }
    public NotificationTypeEnum NotificationTypeEnum { get; init; }
    public IEnumerable<string> UserIds { get; init; } = [];
}