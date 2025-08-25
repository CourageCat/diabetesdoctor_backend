using NotificationService.Contract.Enums;
using NotificationService.Contract.EventBus.Events.Notifications;

namespace NotificationService.Contract.Services.Notification;

public record CreateNotificationCommand : ICommand
{
    public string Title { get; init; }
    public string Body { get; init; }
    public string Thumbnail { get; init; }
    public NotificationTypeEnum NotificationTypeEnum { get; init; }
    public IEnumerable<string> UserIds { get; init; } = [];
    public string SenderId { get; init; }


    public CreateNotificationCommand(string title, string body, string thumbnail, NotificationTypeEnum notificationTypeEnum, IEnumerable<string> userIds, string senderId)
    {
        Title = title;
        Body = body;
        Thumbnail = thumbnail;
        NotificationTypeEnum = notificationTypeEnum;
        UserIds = userIds;
        SenderId = senderId;
    }
}

public record CreatePostNotificationCommand(
    string PostId,
    string Title,
    string Thumbnail,
    NotificationTypeEnum NotificationTypeEnum) : ICommand;

public record DeleteNotificationCommand(string UserId, string NotificationId) : ICommand;
public record MarkReadNotificationCommand(string UserId, List<string> NotificationIds) : ICommand;