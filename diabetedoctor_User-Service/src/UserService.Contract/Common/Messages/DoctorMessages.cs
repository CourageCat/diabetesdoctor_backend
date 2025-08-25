namespace UserService.Contract.Common.Messages;

public enum DoctorMessages
{
    [Message("Tạo bác sĩ thành công.", "doctor_01")]
    CreateDoctorSuccessfully,
    [Message("Cập nhật hồ sơ thành công", "doctor_02")]
    UpdateProfileSuccessfully,
    [Message("Danh sách bác sĩ: ", "doctor_03")]
    GetAllDoctorsSuccessfully,
    [Message("Thông tin chi tiết của bác sĩ ", "doctor_04")]
    GetDoctorByIdSuccessfully,
    [Message("Thay đổi ảnh đại diện thành công", "doctor_05")]
    ChangeAvatarSuccessfully,
    
    [Message("Số điện thoại đã được sử dụng!", "doctor_error_01")]
    PhoneNumberAlreadyExists,
    [Message("Không tìm thấy bác sĩ nào!", "doctor_error_02")]
    DoctorsNotFound,
    [Message("Không tìm thấy bác sĩ!", "doctor_error_03")]
    DoctorNotFound,
    [Message("Hồ sơ không tồn tại.", "doctor_error_04")]
    ProfileNotExist,
}
