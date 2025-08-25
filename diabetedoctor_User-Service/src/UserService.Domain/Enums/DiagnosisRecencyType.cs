namespace UserService.Domain.Enums;

/// <summary>
/// Mức độ gần đây của chẩn đoán bệnh tiểu đường
/// </summary>
public enum DiagnosisRecencyType
{
    // Chẩn đoán trong 3 tháng gần đây
    WITHIN_3_MONTHS,

    // Chẩn đoán trong 6 tháng gần đây
    WITHIN_6_MONTHS,

    // Chẩn đoán cách đây hơn 1 năm
    OVER_1_YEAR
}
