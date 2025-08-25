namespace MediaService.Contract.Common.Messages;

public enum HospitalMessage
{
    [Message("Tên bệnh viện đã tồn tại!", "hospital_01")]
    HospitalNameExistException,

    [Message("Tạo bệnh viện thành công.", "hospital_02")]
    CreateHospitalSuccessfully,
}
