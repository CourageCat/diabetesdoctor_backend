namespace ConsultationService.Contract.Helpers;

public static class CurrentTimeService
{
    private const string VietnamTimeZoneInfo = "SE Asia Standard Time"; 
    public static DateTime GetCurrentTimeUtc() => DateTime.UtcNow;
    public static DateTime GetVietNamCurrentTime() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(VietnamTimeZoneInfo));

}