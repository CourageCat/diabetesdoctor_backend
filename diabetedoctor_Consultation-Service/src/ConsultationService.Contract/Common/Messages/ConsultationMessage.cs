using ConsultationService.Contract.Attributes;

namespace ConsultationService.Contract.Common.Messages;

public enum ConsultationMessage
{
    // handler
    [Message("Đặt lịch tư vấn thành công", "consultation_1")]
    BookConsultationSuccessfully,
    [Message("Hủy lịch tư vấn thành công", "consultation_2")]
    CancelConsultationSuccessfully,
    [Message("Danh sách các lịch tư vấn đã đặt", "consultation_3")]
    GetConsultationHistories,
    
    // validation
    [Message("Không thể đặt lịch vì đã quá thời gian bắt đầu.", "consultation_validation_1")]
    BookingTimeExpired,
    [Message("Không thể hủy lịch: thời gian hủy phải trước 30 phút so với giờ hẹn", "consultation_validation_2")]
    CancelBookingTimeExpired,
    
    // error
    [Message("Không tìm thấy lịch tư vấn", "consultation_error_1")]
    ConsultationNotFound,
    [Message("Không đủ số lượng lượt tư vấn!", "consultation_error_2")]
    ConsultationSessionsNotEnough,
}