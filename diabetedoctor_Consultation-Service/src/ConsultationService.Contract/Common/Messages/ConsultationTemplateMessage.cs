using ConsultationService.Contract.Attributes;

namespace ConsultationService.Contract.Common.Messages;

public enum ConsultationTemplateMessage
{
    // handler
    [Message("Tạo khung giờ tư vấn thành công", "consultation-template_1")]
    CreateTemplatesSuccessfully,
    [Message("Cập nhật khung giờ tư vấn thành công", "consultation-template_2")]
    UpdateTemplateSuccessfully,
    [Message("Xóa khung giờ tư vấn thành công", "consultation-template_3")]
    DeleteTemplateSuccessfully,
    [Message("Danh sách các khung giờ tư vấn của bác sĩ", "consultation-template_4")]
    GetDoctorConsultationTemplates,
    
    
    
    // validation
    [Message("Thời gian tư vấn tối thiểu là 15 phút", "consultation-template_validation_1")]
    MinimumDuration,
    [Message("Thời gian bắt đầu phải trước thời gian kết thúc", "consultation-template_validation_2")]
    StartTimeAfterEndTime,
    [Message("Khung thời gian tư vấn đã được đặt", "consultation-template_validation_3")]
    TemplateIsBooked,
    
    // error
    [Message("Khung giờ tư vấn không tồn tại", "consultation-template_error_1")]
    TemplateNotFound,
}