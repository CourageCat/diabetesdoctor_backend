namespace UserService.Domain.ValueObjects;

/// <summary>
/// Lớp trừu tượng đại diện cho giá trị của một gói dịch vụ.
/// </summary>
/// <remarks>
/// Các lớp kế thừa bao gồm:
/// <list type="bullet">
///     <item><description><see cref="HeightValue"/> – Chiều cao.</description></item>
///     <item><description><see cref="WeightValue"/> – Cân nặng.</description></item>
///     <item><description><see cref="BloodGlucoseValue"/> – Đường huyết.</description></item>
///     <item><description><see cref="BloodPressureValue"/> – Huyết áp.</description></item>
///     <item><description><see cref="HbA1cValue"/> – HbA1c.</description></item>
/// </list>
/// </remarks>
public abstract class PackageFeatureValue : ValueObject
{
}