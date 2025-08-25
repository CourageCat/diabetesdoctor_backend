namespace UserService.Domain.Enums;

public enum TimeOfDayHintsType
{
    None = 0,
    Morning = 1,    // Buổi sáng (6:00 - 9:00)
    Noon = 2,       // Buổi trưa (11:00 - 13:00)
    Afternoon = 4,  // Buổi chiều (13:00 - 17:00)
    Evening = 8,    // Buổi tối (18:00 - 21:00)
    Night = 16      // Buổi đêm (22:00 - 24:00)
}
