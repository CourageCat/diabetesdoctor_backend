namespace UserService.Domain.Enums;

/// <summary>
/// Các loại chỉ số sức khỏe được ghi nhận trong hệ thống
/// </summary>
public enum RecordType
{
    // Chiều cao
    Weight, 
    
    // Cân nặng
    Height, 
    
    // Đường huyết
    BloodGlucose,

    // Huyết áp
    BloodPressure,

    // Chỉ số HbA1c (đường huyết trung bình trong 2–3 tháng)
    HbA1c
}
