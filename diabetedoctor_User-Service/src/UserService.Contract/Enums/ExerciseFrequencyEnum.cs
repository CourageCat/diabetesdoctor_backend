namespace UserService.Contract.Enums;

/// <summary>
/// Tần suất tập thể dục
/// </summary>
public enum ExerciseFrequencyEnum
{
    [Description("Không tập")]
    None,

    [Description("1–2 lần/tuần")]
    OneToTwo,

    [Description("3–5 lần/tuần")]
    ThreeToFive,

    [Description("> 5 lần/tuần")]
    MoreThanFive
}