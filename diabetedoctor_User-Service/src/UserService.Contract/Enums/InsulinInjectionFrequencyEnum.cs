namespace UserService.Contract.Enums;

/// <summary>
/// Tần suất tiêm Insulin
/// </summary>
public enum InsulinInjectionFrequencyEnum
{
    [Description("1 lần/ngày")]
    OncePerDay,

    [Description("2 lần/ngày")]
    TwicePerDay,

    [Description("3 lần/ngày hoặc hơn")]
    ThreeOrMorePerDay
}