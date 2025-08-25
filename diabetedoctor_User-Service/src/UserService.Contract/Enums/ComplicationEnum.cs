namespace UserService.Contract.Enums;

/// <summary>
/// Các loại biến chứng xảy ra ở bệnh nhân tiểu đường
/// </summary>
public enum ComplicationEnum
{
    [Description("Mắt")]
    Eye,

    [Description("Thận")]
    Kidney,

    [Description("Thần kinh")]
    Nerve,

    [Description("Tim mạch")]
    Cardiovascular,

    [Description("Bàn chân")]
    Foot,

    [Description("Khác")]
    Other
}
