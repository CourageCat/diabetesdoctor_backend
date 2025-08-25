namespace MediaService.Contract.Common.Messages;

public enum UserMessage
{
    [Message("Đã lấy thông tin người dùng thành công.", "user_01")]
    GetUserSuccessfully,
    [Message("Tạo thông tin người dùng thành công.", "user_02")]
    CreateUserSuccessfully,
    
    [Message("Không tìm thấy User!", "user_error_01")]
    UserNotFoundException,
}
