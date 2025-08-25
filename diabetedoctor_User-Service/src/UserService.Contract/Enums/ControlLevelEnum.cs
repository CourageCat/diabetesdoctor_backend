namespace UserService.Contract.Enums;

/// <summary>
/// Mức độ kiểm soát chỉ số HbA1c
/// </summary>
public enum ControlLevelEnum
{
    [Description("Tốt (<7%)")]
    GoodControl,

    [Description("Trung bình (7–8%)")]
    ModerateControl,

    [Description("Kém (>8%)")]
    PoorControl,

    [Description("Không rõ")]
    NoInformation
}
