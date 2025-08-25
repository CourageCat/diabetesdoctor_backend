namespace NotificationService.Contract.Common.Messages;

public enum NotificationMessage
{
    [Message("Tất cả thông báo: ", "noti01")]
    GetAllNotificationsSuccessfully,

    [Message("Thông báo không tồn tại", "noti02")]
    NotificationNotFoundException,

    [Message("Xóa thông báo thành công", "noti03")]
    DeletedNotificationSuccessfully,

    [Message("Cập nhật thông báo thành công", "noti04")]
    UpdatedNotificationSuccessfully,

    [Message("Gửi thông báo thành công", "noti05")]
    SendNotificationSuccessfully,
}