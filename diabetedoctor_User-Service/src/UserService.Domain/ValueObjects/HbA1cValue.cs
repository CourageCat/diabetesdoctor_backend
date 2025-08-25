using System.Text.Json.Serialization;

namespace UserService.Domain.ValueObjects;

/// <summary>
/// HbA1c health value (%). Assesses average blood glucose control over 2–3 months.
/// </summary>
public sealed class HbA1cValue : HealthRecordValue
{
    public string Type => "hba1c";
    public double Value { get; }
    public string Unit { get; }
    public HbA1cLevelType Level { get; }

    [JsonConstructor]
    public HbA1cValue(double value, string unit, HbA1cLevelType level)
    {
        Value = value;
        Unit = unit;
        Level = level;
    }

    public static HbA1cValue Of(double value, string unit = "%")
    {
        if (value < 0 || value > 20)
            throw new ArgumentException("HbA1c value must be between 0 and 20%.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit is required.");

        var trimmedUnit = unit.Trim();
        var level = DetermineLevel(value);

        return new HbA1cValue(value, trimmedUnit, level);
    }

    private static HbA1cLevelType DetermineLevel(double value)
    {
        if (value <= 5.6)
            return HbA1cLevelType.Ideal;

        if (value <= 6.4)
            return HbA1cLevelType.Normal;

        if (value <= 7.9)
            return HbA1cLevelType.High;

        return HbA1cLevelType.VeryHigh;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
        yield return Level;
    }
}
