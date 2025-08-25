using NotificationService.Contract.DTOs.NotificationDtos;

namespace NotificationService.Contract.DTOs.Responses;

public class GetNotificationsResponse
{
    public PagedList<NotificationDto> Notifications { get; init; } = default!;
}