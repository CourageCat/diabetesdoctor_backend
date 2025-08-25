using System.Text.Json.Serialization;

namespace UserService.Domain.ValueObjects;

/// <summary>
/// Giá trị huyết áp (blood pressure), gồm huyết áp tâm thu và tâm trương, đơn vị mmHg.
/// </summary>
public sealed class BloodPressureValue : HealthRecordValue
{
    public string Type => "blood_pressure";
    public double Systolic { get; } // Huyết áp tâm thu
    public double Diastolic { get; } // Huyết áp tâm trương
    public string Unit { get; } // Đơn vị
    public BloodPressureLevelType Level { get; } // Mức độ

    private const double MinSystolic = 70;
    private const double MaxSystolic = 200;
    private const double MinDiastolic = 40;
    private const double MaxDiastolic = 110;

    [JsonConstructor]
    public BloodPressureValue(double systolic, double diastolic, string unit, BloodPressureLevelType level)
    {
        Systolic = systolic;
        Diastolic = diastolic;
        Unit = unit;
        Level = level;
    }

    private static BloodPressureLevelType DetermineLevel(double systolic, double diastolic)
    {
        if (systolic > 180 || diastolic > 110)
            return BloodPressureLevelType.HypertensionStage3;

        if (systolic >= 160 || diastolic >= 100)
            return BloodPressureLevelType.HypertensionStage2;

        if (systolic >= 140 || diastolic >= 90)
            return BloodPressureLevelType.HypertensionStage1;

        if (systolic >= 130 || diastolic >= 85)
            return BloodPressureLevelType.HighNormal;

        if (systolic >= 120 || diastolic >= 80)
            return BloodPressureLevelType.Normal;

        return BloodPressureLevelType.Low;
    }

    public static BloodPressureValue Of(double systolic, double diastolic, string unit = "mmHg")
    {
        if (systolic <= 0 || diastolic <= 0)
            throw new ArgumentException("Chỉ số huyết áp không được âm hoặc bằng 0.");

        if (systolic < diastolic)
            throw new ArgumentException("Huyết áp tâm thu phải lớn hơn hoặc bằng huyết áp tâm trương.");

        if (systolic < MinSystolic || systolic > MaxSystolic)
            throw new ArgumentException($"Huyết áp tâm thu phải nằm trong khoảng {MinSystolic} - {MaxSystolic} {unit}.");

        if (diastolic < MinDiastolic || diastolic > MaxDiastolic)
            throw new ArgumentException($"Huyết áp tâm trương phải nằm trong khoảng {MinDiastolic} - {MaxDiastolic} {unit}.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Đơn vị bắt buộc.");
        
        var trimmedUnit = unit.Trim();
        var level = DetermineLevel(systolic, diastolic);

        return new BloodPressureValue(systolic, diastolic, trimmedUnit, level);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Systolic;
        yield return Diastolic;
        yield return Unit;
    }
}
