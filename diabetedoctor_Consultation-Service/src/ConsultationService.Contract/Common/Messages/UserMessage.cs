using ConsultationService.Contract.Attributes;

namespace ConsultationService.Contract.Common.Messages;

public enum UserMessage
{
    [Message("Danh sách người dùng", "user_1")]
    AvailableUsers,
    
    
    // Error
    [Message("Người dùng không tồn tại hoặc đã bị cấm khỏi hệ thống", "user_er_1")]
    UserNotFound,
    
    [Message("Nhân viên không tồn tại hoặc đã bị cấm khỏi hệ thống", "user_er_1")]
    StaffNotFound,
    
    [Message("Role không phù hợp", "user_er_2")]
    MustHaveThisRole,
    
    [Message("Bác sĩ không trực thuộc bệnh viện", "user_er_3")]
    DoctorNotBelongToHospital,
    
    [Message("Nhân viên không trực thuộc bệnh viện", "user_er_04")]
    StaffNotBelongToHospital
}
