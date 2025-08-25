namespace UserService.Contract.Enums;

public enum BloodGlucoseLevelEnum
{
    [Description("Rất cao")]
    VeryHigh,

    [Description("Cao")]
    High,

    [Description("Bình thường")]
    Normal,

    [Description("Thấp")]
    Low,

    [Description("Rất thấp")]
    VeryLow
}
