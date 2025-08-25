namespace UserService.Contract.Enums;

public enum BloodGlucoseMeasureTimeEnum
{
    [Description("Lúc đói")]
    Fasting,

    [Description("Trước bữa ăn")]
    BeforeMeal,

    [Description("Sau bữa ăn")]
    AfterMeal,
    [Description("Trước khi ngủ")]
    BeforeBed
}