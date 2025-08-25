namespace UserService.Contract.Common.Messages;

public enum CarePlanInstanceMessages
{
    [Message("Tạo lịch nhắc nhở uống thuốc thành công.", "care_plan_instance_01")]
    CreateCarePlanInstanceSuccessfully,
    [Message("Cập nhật lịch nhắc nhở uống thuốc thành công.", "care_plan_instance_02")]
    UpdateCarePlanInstanceSuccessfully,
    [Message("Xóa lịch nhắc nhở uống thuốc thành công.", "care_plan_instance_03")]
    DeleteCarePlanInstanceSuccessfully,
    
    [Message("Lịch đo với chỉ số và thời điểm này đã bị trùng!", "care_plan_instance_error_01")]
    DuplicatedCarePlanInstance,
    [Message("Lịch đo không tồn tại!", "care_plan_instance_error_02")]
    CarePlanInstanceNotExist,
    [Message("Lịch đo không thuộc về bệnh nhân này!", "care_plan_instance_error_03")]
    CarePlanInstanceNotBelongToPatient,
    [Message("Lịch đo không thuộc của bác sĩ này tạo!", "care_plan_instance_error_04")]
    CarePlanInstanceNotBelongToDoctor,
    [Message("Không được cập nhật lịch đo của bác sĩ tạo!", "care_plan_instance_error_05")]
    CanNotUpdateCarePlanInstanceBelongToDoctor,
    [Message("Không được xóa lịch đo của bác sĩ tạo!", "care_plan_instance_error_06")]
    CanNotDeleteCarePlanInstanceBelongToDoctor,
    [Message("Không được cập nhật lịch đo của bệnh nhân tạo!", "care_plan_instance_error_07")]
    CanNotUpdateCarePlanInstanceBelongToPatient,
    [Message("Không được xóa lịch đo của bệnh nhân tạo!", "care_plan_instance_error_08")]
    CanNotDeleteCarePlanInstanceBelongToPatient,
}