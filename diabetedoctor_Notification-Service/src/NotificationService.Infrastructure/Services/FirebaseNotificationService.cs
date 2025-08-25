using NotificationService.Contract.DTOs;
using NotificationService.Contract.DTOs.FcmDtos;
using NotificationService.Contract.Infrastructure;

namespace NotificationService.Infrastructure.Services;

public class FirebaseNotificationService<T> : IFirebaseNotificationService<T> where T : BaseFcmDto
{
    public async Task PushNotificationMultiDeviceAsync(List<string> deviceIds, T notification)
    {
        for (var i = 0; i < deviceIds.Count; i += 450)
        {
            var currentBatch = deviceIds
                .Skip(i)
                .Take(450)
                .ToList();
            
            var message = new MulticastMessage()
            {
                Tokens = currentBatch,
                Notification = new Notification
                {
                    Title = notification.Title,
                    Body = notification.Body
                },
                Data = notification.GetData(),
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    CollapseKey = "com.quyndc.diadoc",
                    TimeToLive = TimeSpan.FromDays(28),
                    // Notification = new AndroidNotification
                    // {
                    //     ImageUrl = notification.Icon
                    // }
                }
            };
            await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
        }
    }
}

