namespace UserService.Contract.Common.Messages;

public enum HospitalAdminMessages
{
    [Message("Tạo quản trị viên bệnh viện thành công.", "hospital_admin_01")]
    CreateHospitalAdminSuccessfully,
    [Message("Cập nhật hồ sơ thành công", "hospital_admin_02")]
    UpdateProfileSuccessfully,
    [Message("Danh sách quản trị viên bệnh viện: ", "hospital_admin_03")]
    GetAllHospitalAdminsSuccessfully,
    [Message("Thông tin chi tiết của nhân viên bệnh viện ", "hospital_admin_04")]
    GetHospitalAdminByIdSuccessfully,
    [Message("Thay đổi ảnh đại diện thành công", "hospital_admin_05")]
    ChangeAvatarSuccessfully,
    
    [Message("Không tìm thấy nhân viên bệnh viện này!", "hospital_admin_error_01")]
    HospitalAdminNotFound,
    [Message("Email đã được sử dụng!", "hospital_admin_error_02")]
    EmailAlreadyExists,
    [Message("Hồ sơ không tồn tại.", "hospital_admin_error_03")]
    ProfileNotExist,
}