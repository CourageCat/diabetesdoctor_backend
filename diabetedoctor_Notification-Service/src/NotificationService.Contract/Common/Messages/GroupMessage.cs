namespace NotificationService.Contract.Common.Messages;

public enum GroupMessage
{
    [Message("Nhóm chat không tồn tại.", "group01")]
    GroupNotFoundException,

    [Message("Cập nhật nhóm chat thành công", "group02")]
    UpdatedGroupSuccessfully,

    [Message("Xóa nhóm chat thành công", "group03")]
    DeletedGroupSuccessfully,

    [Message("Xóa thành viên khỏi nhóm chat thành công", "group04")]
    GroupMemberRemovedSuccessfully,
}