namespace UserService.Domain.ValueObjects;

/// <summary>
/// Giá trị đại diện cho thông tin thời điểm chẩn đoán bệnh.
/// Bao gồm năm được chẩn đoán (nếu có) và mức độ gần đây.
/// </summary>
public sealed class DiagnosisInfo : ValueObject
{
    public int? Year { get; } // Năm
    
    // Loại thời gian chẩn đoán (dưới 3 tháng, dưới 6 tháng, từ 1 năm trở lên)
    public DiagnosisRecencyType DiagnosisRecency { get; } 

    private DiagnosisInfo() { }

    private DiagnosisInfo(DiagnosisRecencyType diagnosisRecency, int? year)
    {
        DiagnosisRecency = diagnosisRecency;
        Year = year;
    }

    public static DiagnosisInfo Of(DiagnosisRecencyType diagnosisRecency, int? year)
    {
        return new DiagnosisInfo(diagnosisRecency, year);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Year;
        yield return DiagnosisRecency;
    }
}
