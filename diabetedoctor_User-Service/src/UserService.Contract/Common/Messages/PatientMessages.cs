namespace UserService.Contract.Common.Messages;

public enum PatientMessages
{
    [Message("Tạo hồ sơ thành công", "patient_01")]
    CreateProfileSuccessfully,
    [Message("Cập nhật hồ sơ thành công", "patient_02")]
    UpdateProfileSuccessfully,
    [Message("Chi tiết hồ sơ: ", "patient_03")]
    GetPatientProfileSuccessfully,
    [Message("Thay đổi ảnh đại diện thành công", "patient_04")]
    ChangeAvatarSuccessfully,
    [Message("Số lượt tư vấn của bệnh nhân: ", "patient_05")]
    GetConsultationSessionsSuccessfully,
    [Message("Sử dụng lượt tư vấn thành công", "patient_05")]
    UseConsultationSessionsSuccessfully,
    [Message("Tất cả bác sĩ đã tạo mẫu lịch cho bệnh nhân: ", "patient_06")]
    GetAllDoctorsCreatedTemplateSuccessfully,
    
    [Message("Hồ sơ đã tồn tại.", "patient_error_01")]
    ProfileExist,
    [Message("Hồ sơ không tồn tại.", "patient_error_02")]
    ProfileNotExist,
    [Message("Chỉ số đã được cập nhật rồi", "patient_error_03")]
    HealthRecordExist,
    [Message("Số điện thoại đã tồn tại!", "patient_error_04")]
    PhoneNumberExist,
    [Message("Số điện thoại chưa đăng kí!", "patient_error_05")]
    PhoneNumberNotRegistered,
    [Message("Thay đổi ảnh đại diện thất bại!", "patient_error_06")]
    ChangeAvatarFailed,
    [Message("Không tìm thấy số lượt tư vấn!", "patient_error_07")]
    ConsultationSessionsNotFound,
    [Message("Không đủ số lượng lượt tư vấn!", "patient_error_08")]
    ConsultationSessionsNotEnough,
}
