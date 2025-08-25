namespace UserService.Contract.Common.Messages;

public enum CarePlanTemplateMessages
{
    [Message("Tạo mẫu lịch nhắc nhở uống thuốc thành công.", "care_plan_template_01")]
    CreateCarePlanTemplateSuccessfully,
    [Message("Cập nhật mẫu lịch nhắc nhở uống thuốc thành công.", "care_plan_template_02")]
    UpdateCarePlanTemplateSuccessfully,
    [Message("Xóa mẫu lịch nhắc nhở uống thuốc thành công.", "care_plan_template_03")]
    DeleteCarePlanTemplateSuccessfully,
    [Message("Danh sách mẫu lịch đo: ", "care_plan_template_04")]
    GetAllCarePlanTemplatesSuccessfully,
    
    [Message("Mẫu lịch đo với chỉ số và thời điểm này đã bị trùng!", "care_plan_template_error_01")]
    DuplicatedCarePlanTemplate,
    [Message("Mẫu lịch đo không tồn tại!", "care_plan_template_error_02")]
    CarePlanTemplateNotExist,
    [Message("Mẫu lịch đo không thuộc về bệnh nhân này!", "care_plan_template_error_03")]
    CarePlanTemplateNotBelongToPatient,
    [Message("Mẫu lịch đo phải của bác sĩ này tạo!", "care_plan_template_error_04")]
    CarePlanTemplateNotBelongToDoctor,
    [Message("Không được cập nhật mẫu lịch đo của bác sĩ tạo!", "care_plan_template_error_05")]
    CanNotUpdateCarePlanTemplateBelongToDoctor,
    [Message("Không được xóa mẫu lịch đo của bác sĩ tạo!", "care_plan_template_error_06")]
    CanNotDeleteCarePlanTemplateBelongToDoctor,
    [Message("Không được cập nhật mẫu lịch đo của bệnh nhân tạo!", "care_plan_template_error_07")]
    CanNotUpdateCarePlanTemplateBelongToPatient,
    [Message("Không được xóa mẫu lịch đo của bệnh nhân tạo!", "care_plan_template_error_08")]
    CanNotDeleteCarePlanTemplateBelongToPatient,
}