namespace UserService.Contract.Common.Messages;

public enum HealthRecordMessages
{
    [Message("Tạo chỉ số sức khỏe thành công.", "health_record_01")]
    CreateHealthRecordSuccessfully,
    [Message("Tạo ghi chú từ AI thành công.", "health_record_02")]
    UpdateAiNoteSuccessfully,
    
    [Message("Không tìm thấy chỉ số sức khỏe này!", "health_record_error_01")]
    HealthRecordNotFound,
    [Message("Chỉ số sức khỏe không phải của người dùng này!", "health_record_error_02")]
    HealthRecordNotBelongToUser,
    [Message("Cập nhật ghi chú từ AI thất bại!", "health_record_error_03")]
    GenerateAiNoteFailed,
}