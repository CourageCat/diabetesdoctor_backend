namespace UserService.Contract.Common.Messages;

public enum AdminMessages
{
    [Message("Không tìm thấy Admin này!", "admin_error_01")]
    AdminNotFound,
}