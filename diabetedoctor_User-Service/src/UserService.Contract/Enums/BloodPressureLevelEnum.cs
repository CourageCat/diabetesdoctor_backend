namespace UserService.Contract.Enums;

public enum BloodPressureLevelEnum
{
    [Description("Huyết áp thấp")]
    Low,

    [Description("Bình thường")]
    Normal,

    [Description("Bình thường cao")]
    HighNormal,

    [Description("Tăng huyết áp độ 1")]
    HypertensionStage1,

    [Description("Tăng huyết áp độ 2")]
    HypertensionStage2,

    [Description("Tăng huyết áp độ 3")]
    HypertensionStage3
}
