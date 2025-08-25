namespace UserService.Contract.Common.Messages;

public enum HospitalMessages
{
    [Message("Tạo bệnh viện thành công.", "hospital_01")]
    CreateHospitalSuccessfully,
    [Message("Cập nhật bệnh viện thành công.", "hospital_02")]
    UpdateHospitalSuccessfully,
    [Message("Danh sách bệnh viện: ", "hospital_03")]
    GetAllHospitalsSuccessfully,
    [Message("Thông tin chi tiết của bệnh viện ", "hospital_04")]
    GetHospitalByIdSuccessfully,
    
    [Message("Số điện thoại đã được sử dụng!", "hospital_error_01")]
    PhoneNumberAlreadyExists,
    [Message("Email đã được sử dụng!", "hospital_error_02")]
    EmailAlreadyExists,
    [Message("Không tìm thấy bệnh viện nào!", "hospital_error_03")]
    HospitalsNotFound,
    [Message("Không tìm thấy bệnh viện! ", "hospital_error_04")]
    HospitalNotFound
}