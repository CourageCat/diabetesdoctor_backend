namespace UserService.Contract.Common.Messages;

public enum HospitalStaffMessages
{
    [Message("Tạo nhân viên bệnh viện thành công.", "hospital_staff_01")]
    CreateHospitalStaffSuccessfully,
    [Message("Cập nhật hồ sơ thành công", "hospital_staff_02")]
    UpdateProfileSuccessfully,
    [Message("Danh sách nhân viên bệnh viện: ", "hospital_staff_03")]
    GetAllHospitalStaffsSuccessfully,
    [Message("Thông tin chi tiết của nhân viên bệnh viện ", "hospital_staff_04")]
    GetHospitalStaffByIdSuccessfully,
    [Message("Thay đổi ảnh đại diện thành công", "hospital_staff_05")]
    ChangeAvatarSuccessfully,
    
    [Message("Không tìm thấy nhân viên bệnh viện này!", "hospital_staff_error_01")]
    HospitalStaffNotFound,
    [Message("Email đã được sử dụng!", "hospital_staff_error_02")]
    EmailAlreadyExists,
    [Message("Hồ sơ không tồn tại.", "hospital_staff_error_03")]
    ProfileNotExist,
}