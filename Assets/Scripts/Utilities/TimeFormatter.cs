using System.Collections.Generic;

namespace Utilities
{
    public class TimeFormatter
    {
        public bool ShowHours = false;
        public bool ShowMinutes = true;
        public bool ShowSeconds = true;
        public bool ShowMilliseconds = false;

        public bool UseUnits = false;
        public bool LeadingZeros = true;
        public bool ShowEmpty = false;

        public string Separator = ":";
    }

    public static class TimeFormatterExtensions
    {
        public static string FormatToTime(this double timeInSeconds, TimeFormatter format = null)
        {
            format ??= new TimeFormatter();
            var totalMilliseconds = (int)(timeInSeconds * 1000);

            var hours = totalMilliseconds / 3600000;
            var minutes = (totalMilliseconds % 3600000) / 60000;
            var seconds = (totalMilliseconds % 60000) / 1000;
            var milliseconds = totalMilliseconds % 1000;

            var parts = new List<string>();
            if (format.ShowHours && !(format.ShowEmpty && hours == 0))
                parts.Add(FormatUnit(hours, format.UseUnits ? "h" : "", format.LeadingZeros ? 2 : 0));
            if (format.ShowMinutes && !(format.ShowEmpty && minutes == 0))
                parts.Add(FormatUnit(minutes, format.UseUnits ? "m" : "", format.LeadingZeros ? 2 : 0));
            if (format.ShowSeconds && !(format.ShowEmpty && seconds == 0))
                parts.Add(FormatUnit(seconds, format.UseUnits ? "s" : "", format.LeadingZeros ? 2 : 0));
            if (format.ShowMilliseconds && !(format.ShowEmpty && milliseconds == 0))
                parts.Add(FormatUnit(milliseconds, format.UseUnits ? "ms" : "", format.LeadingZeros ? 3 : 0));

            return string.Join(format.Separator, parts);
        }

        public static string FormatToTime(this float timeInSeconds, TimeFormatter format = null)
        {
            return FormatToTime((double)timeInSeconds, format);
        }

        public static string FormatToTime(this int timeInSeconds, TimeFormatter format = null)
        {
            return FormatToTime((double)timeInSeconds, format);
        }

        private static string FormatUnit(int value, string unit = "", int leadingZeros = 0)
        {
            return $"{value.ToString().PadLeft(leadingZeros, '0')}{unit}";
        }
    }
}
