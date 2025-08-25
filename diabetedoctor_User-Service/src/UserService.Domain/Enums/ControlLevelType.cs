namespace UserService.Domain.Enums;

/// <summary>
/// Mức độ kiểm soát chỉ số HbA1c của bệnh nhân tiểu đường
/// </summary>
public enum ControlLevelType
{
    // Kiểm soát tốt (HbA1c < 7%)
    GoodControl,

    // Kiểm soát trung bình (HbA1c từ 7–8%)
    ModerateControl,

    // Kiểm soát kém (HbA1c > 8%)
    PoorControl,

    // Không có thông tin
    NoInformation
}