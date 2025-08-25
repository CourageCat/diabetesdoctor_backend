namespace UserService.Contract.Enums;

/// <summary>
/// Mốc thời gian chẩn đoán bệnh
/// </summary>
public enum DiagnosisRecencyEnum
{
    [Description("≤ 3 tháng")]
    WITHIN_3_MONTHS,

    [Description("≤ 6 tháng")]
    WITHIN_6_MONTHS,

    [Description("≥ 1 năm")]
    OVER_1_YEAR
}
