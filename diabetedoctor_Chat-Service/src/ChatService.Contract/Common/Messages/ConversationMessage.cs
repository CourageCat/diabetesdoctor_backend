using ChatService.Contract.Attributes;

namespace ChatService.Contract.Common.Messages;

public enum ConversationMessage
{
    // handler
    [Message("Tạo nhóm thành công", "conversation_1")]
    CreatedGroupSuccessfully,
    [Message("Cập nhật thành công", "conversation_2")]
    UpdatedGroupSuccessfully,
    [Message("Xóa nhóm thành công", "conversation_3")]
    DeletedGroupSuccessfully,
    [Message("Thêm thành viên thành công", "conversation_4")]
    AddMemberToGroupSuccessfully,
    [Message("Xóa thành viên thành công", "conversation_5")]
    RemoveMemberFromGroupSuccessfully,
    [Message("Thêm bác sĩ thành công", "conversation_6")]
    AddDoctorToGroupSuccessfully,
    [Message("Danh sách nhóm trò chuyện của người dùng", "conversation_7")]
    UserGroupConversations,
    [Message("Danh sách nhóm trò chuyện của bệnh viện", "conversation_8")]
    HospitalConversations,
    [Message("Thông tin nhóm trò chuyện", "conversation_9")]
    ConversationInDetail,
    [Message("Rời nhóm thành công", "conversation_10")]
    LeaveGroupConversationSuccessfully,
    [Message("Tham gia nhóm thành công", "conversation_11")]
    JoinGroupSuccessfully,
    // validation
    [Message("Tên mới trùng với tên cũ", "conversation_validation_1")]
    SameAsCurrentName,
    
    // error
    [Message("Không tìm thấy cuộc trò chuyện hoặc bạn không phải thành viên trong nhóm.", "conversation_er_01")]
    ConversationNotFound,
    [Message("Không tìm thấy cuộc trò chuyện hoặc bạn không có quyền truy cập.", "conversation_er_01")]
    ConversationNotFoundOrAccessDenied,
    [Message("Không tìm thấy cuộc trò chuyện", "conversation_er_01")]
    ConversationDetailNotFound,
    [Message("Không có quyền để thực hiện thao tác này", "conversation_er_02")]
    ConversationAccessDenied,
    [Message("Không có quyền để xóa thành viên này", "conversation_er_05")]
    CannotRemoveMember,
    [Message("Cuộc trò chuyện này đã đóng", "conversation_er_07")]
    ThisConversationIsClosed,
    [Message("Bạn không có quyền truy cập cuộc trò chuyện này vì không thuộc bệnh viện bạn làm việc.", "conversation_er_07")]
    ThisConversationNotBelongToYourHospital,
}