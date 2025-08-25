namespace UserService.Contract.Enums;

/// <summary>
/// Thói quen ăn uống
/// </summary>
public enum EatingHabitEnum
{
    [Description("Kiêng nghiêm ngặt")]
    StrictDiet,

    [Description("Bình thường")]
    Normal,

    [Description("Nhiều ngọt/tinh bột")]
    SweetCarbHeavy
}
