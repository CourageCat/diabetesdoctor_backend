namespace UserService.Domain.Enums;

/// <summary>
/// Tần suất tập thể dục của bệnh nhân
/// </summary>
public enum ExerciseFrequencyType
{
    // Không tập thể dục
    None,

    // Tập 1–2 lần mỗi tuần
    OneToTwo,

    // Tập 3–5 lần mỗi tuần
    ThreeToFive,

    // Tập thể dục hơn 5 lần mỗi tuần
    MoreThanFive
}
