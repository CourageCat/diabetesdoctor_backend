namespace UserService.Contract.Enums;

public enum RecordEnum
{
    [Description("Cân nặng")]
    Weight,

    [Description("Chiều cao")]
    Height,

    [Description("Đường huyết")]
    BloodGlucose,
    
    [Description("Huyết áp")]
    BloodPressure,

    [Description("Chỉ số HbA1c (đường huyết trung bình 2-3 tháng)")]
    HbA1c,
}
