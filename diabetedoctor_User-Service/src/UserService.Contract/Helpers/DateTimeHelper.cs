namespace UserService.Contract.Helpers;

public static class DateTimeHelper
{
    public static DateTime ToLocalTimeNow(NationEnum nation)
    {
        return nation switch
        {
            NationEnum.VietNam => DateTime.UtcNow.AddHours(7),
        };
    }
    
    public static DateTime ToLocalTime(NationEnum nation, DateTime utcTime)
    {
        return nation switch
        {
            NationEnum.VietNam => utcTime.AddHours(7),
        };
    }

    public static DateTime ToUtcTime(NationEnum nation, DateTime localTime)
    {
        return nation switch
        {
            NationEnum.VietNam => localTime.AddHours(-7),
        };
    }
    
}