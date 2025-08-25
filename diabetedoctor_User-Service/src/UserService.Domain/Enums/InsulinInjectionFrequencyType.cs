namespace UserService.Domain.Enums;

/// <summary>
/// Tần suất tiêm insulin mỗi ngày
/// </summary>
public enum InsulinInjectionFrequencyType
{
    // 1 lần/ngày
    OncePerDay,

    // 2 lần/ngày
    TwicePerDay,

    // 3 lần/ngày hoặc hơn
    ThreeOrMorePerDay
}