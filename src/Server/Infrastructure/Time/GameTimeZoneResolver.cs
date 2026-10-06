namespace GameNet.Server.Infrastructure.Time;

public static class GameTimeZoneResolver
{
    public static TimeZoneInfo Resolve(string id)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
        catch (TimeZoneNotFoundException)
        {
            if (OperatingSystem.IsWindows() &&
                string.Equals(id, "Asia/Tehran", StringComparison.OrdinalIgnoreCase))
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            }

            if (!OperatingSystem.IsWindows() &&
                string.Equals(id, "Iran Standard Time", StringComparison.OrdinalIgnoreCase))
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran");
            }

            if (OperatingSystem.IsWindows() &&
                TimeZoneInfo.TryConvertIanaIdToWindowsId(id, out var windowsId))
            {
                return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
            }

            if (!OperatingSystem.IsWindows() &&
                TimeZoneInfo.TryConvertWindowsIdToIanaId(id, out var ianaId))
            {
                return TimeZoneInfo.FindSystemTimeZoneById(ianaId);
            }

            throw;
        }
    }
}
