using NotificationService.Contract.DTOs.Responses;

namespace NotificationService.Contract.Services.Notification;

public record GetNotificationsByUserIdQuery : IQuery<GetNotificationsResponse>
{
    public string UserId { get; init; } = string.Empty;
    public QueryFilter Filter { get; init; }
}