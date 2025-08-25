using NotificationService.Contract.DTOs.FcmDtos;

namespace NotificationService.Contract.Infrastructure;

public interface IFirebaseNotificationService<in T> where T : BaseFcmDto
{
    Task PushNotificationMultiDeviceAsync(List<string> deviceIds, T notification);
}