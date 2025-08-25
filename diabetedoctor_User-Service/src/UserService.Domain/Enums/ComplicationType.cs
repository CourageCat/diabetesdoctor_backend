namespace UserService.Domain.Enums;

/// <summary>
/// Các loại biến chứng có thể xảy ra ở bệnh nhân tiểu đường
/// </summary>
public enum ComplicationType
{
    // Biến chứng mắt, như bệnh võng mạc
    Eye,

    // Biến chứng ở thận (ví dụ: suy thận)
    Kidney,

    // Biến chứng thần kinh (tê chân tay, mất cảm giác)
    Nerve,

    // Biến chứng tim mạch (cao huyết áp, đột quỵ)
    Cardiovascular,

    // Biến chứng bàn chân (loét, nhiễm trùng, hoại tử)
    Foot,

    // Biến chứng khác không nằm trong các loại trên
    Other
}
