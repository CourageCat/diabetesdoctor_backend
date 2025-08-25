namespace UserService.Contract.Enums;

public enum HbA1cLevelEnum
{
    [Description("Lý tưởng")]
    Ideal,

    [Description("Tốt")]
    Normal,

    [Description("Cao")]
    High,

    [Description("Rất cao")]
    VeryHigh
}