namespace ManagerAttendance.Common;

public static class TimeZoneHelper
{
    private static readonly Lazy<TimeZoneInfo> _vietnamTimeZone = new Lazy<TimeZoneInfo>(() =>
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }
        catch
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
            catch
            {
                return TimeZoneInfo.CreateCustomTimeZone("SE Asia Standard Time", TimeSpan.FromHours(7), "SE Asia Standard Time", "SE Asia Standard Time");
            }
        }
    });

    /// <summary>
    /// Vietnam Time Zone (UTC+7)
    /// </summary>
    public static TimeZoneInfo VietnamTimeZone => _vietnamTimeZone.Value;

    /// <summary>
    /// Converts a UTC DateTime to Vietnam Time (UTC+7)
    /// </summary>
    public static DateTime GetVietnamTime(DateTime utcDateTime)
    {
        var utc = utcDateTime.Kind == DateTimeKind.Utc
            ? utcDateTime
            : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, VietnamTimeZone);
    }

    /// <summary>
    /// Returns the current DateTime in Vietnam Time (UTC+7)
    /// </summary>
    public static DateTime CurrentVietnamTime => GetVietnamTime(DateTime.UtcNow);

    /// <summary>
    /// Calculates the UTC start (00:00:00) of today in Vietnam timezone.
    /// </summary>
    public static DateTime GetTodayStartUtcInVietnam()
    {
        var vnNow = CurrentVietnamTime;
        var vnTodayStart = vnNow.Date; // 00:00:00 in Vietnam
        return TimeZoneInfo.ConvertTimeToUtc(vnTodayStart, VietnamTimeZone);
    }
}
