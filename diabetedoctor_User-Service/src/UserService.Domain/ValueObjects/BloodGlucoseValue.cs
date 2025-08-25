using System.Text.Json.Serialization;

namespace UserService.Domain.ValueObjects;

/// <summary>
/// Giá trị sức khỏe đường huyết (mmol/L)
/// </summary>
public sealed class BloodGlucoseValue : HealthRecordValue
{
    public string Type => "blood_glucose";
    public double Value { get; }
    public string Unit { get; }
    public BloodGlucoseLevelType Level { get; }
    public BloodGlucoseMeasureTimeType MeasureTimeType { get; }

    [JsonConstructor]
    public BloodGlucoseValue(double value, string unit, BloodGlucoseLevelType level, BloodGlucoseMeasureTimeType measureTimeType)
    {
        Value = value;
        Unit = unit;
        Level = level;
        MeasureTimeType = measureTimeType;
    }

    public static BloodGlucoseValue Of(double value, BloodGlucoseMeasureTimeType measureTimeType, string unit = "mmol/L")
    {
        if (value < 0)
            throw new ArgumentException("Giá trị đường huyết không được âm.");

        var level = AssessLevel(value, measureTimeType);
        return new BloodGlucoseValue(value, unit, level, measureTimeType);
    }

    private static BloodGlucoseLevelType AssessLevel(double mmol, BloodGlucoseMeasureTimeType timeType)
    {
        return timeType switch
        {
            BloodGlucoseMeasureTimeType.Fasting or BloodGlucoseMeasureTimeType.BeforeMeal => mmol switch
            {
                < 3.0 => BloodGlucoseLevelType.VeryLow,
                < 3.9 => BloodGlucoseLevelType.Low,
                <= 7.2 => BloodGlucoseLevelType.Normal,
                <= 10.0 => BloodGlucoseLevelType.High,
                _ => BloodGlucoseLevelType.VeryHigh
            },

            BloodGlucoseMeasureTimeType.AfterMeal => mmol switch
            {
                < 3.0 => BloodGlucoseLevelType.VeryLow,
                < 3.9 => BloodGlucoseLevelType.Low,
                <= 9.9 => BloodGlucoseLevelType.Normal,
                <= 11.0 => BloodGlucoseLevelType.High,
                _ => BloodGlucoseLevelType.VeryHigh
            },

            _ => throw new ArgumentOutOfRangeException(nameof(timeType))
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
        yield return Level;
        yield return MeasureTimeType;
    }
}
