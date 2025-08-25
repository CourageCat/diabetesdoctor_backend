namespace NotificationService.Contract.Common.Messages
{
    public enum UserMessage
    {
        [Message("Không tìm thấy người dùng ", "user01")]
        UserNotFoundException,

        [Message("Không tìm thấy thiết bị nào ", "user02")]
        DeviceNotFoundException,

        [Message("Tạo user thành công", "user03")]
        CreateUserSuccessfully,

        [Message("Cập nhật user thành công", "user04")]
        UpdateUserSuccessfully,
    }
}