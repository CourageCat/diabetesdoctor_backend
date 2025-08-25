namespace UserService.Contract.Enums;

/// <summary>
/// Các loại bệnh nền thường gặp ở bệnh nhân tiểu đường
/// </summary>
public enum MedicalHistoryForDiabetesEnum
{
    [Description("Tăng huyết áp")]
    HYPERTENSION,

    [Description("Rối loạn mỡ máu")]
    DYSPLIPIDEMIA,

    [Description("Bệnh tim mạch")]
    CARDIOVASCULAR_DISEASE,

    [Description("Tai biến mạch máu não")]
    STROKE,

    [Description("Thận mạn")]
    CHRONIC_KIDNEY_DISEASE,

    [Description("Gan mạn tính")]
    CHRONIC_LIVER_DISEASE,

    [Description("Hen suyễn / COPD")]
    ASTHMA_COPD,

    [Description("Béo phì")]
    OBESITY,

    [Description("Tuyến giáp")]
    THYROID_DISORDER,

    [Description("Gút")]
    GOUT,

    [Description("Bệnh mắt")]
    EYE_DISEASE,

    [Description("Thần kinh")]
    NEUROPATHY,

    [Description("Trầm cảm / Lo âu")]
    DEPRESSION,

    [Description("Ung thư")]
    CANCER,

    [Description("Hút thuốc")]
    SMOKING,

    [Description("Rượu bia")]
    ALCOHOL,

    [Description("Khác")]
    OTHER
}
