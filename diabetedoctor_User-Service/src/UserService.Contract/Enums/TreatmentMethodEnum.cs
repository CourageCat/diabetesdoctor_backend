namespace UserService.Contract.Enums;

/// <summary>
/// Phương pháp điều trị dành cho bệnh nhân tiểu đường
/// </summary>
public enum TreatmentMethodEnum
{
    [Description("Tiêm Insulin")]
    InsulinInjection,

    [Description("Thuốc uống")]
    OralMedication,

    [Description("Ăn uống & tập luyện")]
    DietAndExercise
}
