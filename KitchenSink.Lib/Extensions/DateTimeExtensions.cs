using System;
using KitchenSink.Timekeeping;

namespace KitchenSink.Extensions
{
    public static class DateTimeExtensions
    {
        public static TimeSpan Days(this int x)
        {
            return new TimeSpan(x, 0, 0, 0);
        }

        public static TimeSpan Hours(this int x)
        {
            return new TimeSpan(x, 0, 0);
        }

        public static TimeSpan Minutes(this int x)
        {
            return new TimeSpan(0, x, 0);
        }

        public static TimeSpan Seconds(this int x)
        {
            return new TimeSpan(0, 0, x);
        }

        public static DateTime AgoLocal(this TimeSpan x)
        {
            return DateTime.Now.Add(x.Negate());
        }

        public static DateTime AgoUtc(this TimeSpan x)
        {
            return DateTime.UtcNow.Add(x.Negate());
        }

        public static DateTime AheadLocal(this TimeSpan x)
        {
            return DateTime.Now.Add(x);
        }

        public static DateTime AheadUtc(this TimeSpan x)
        {
            return DateTime.UtcNow.Add(x);
        }

        public static DateSpan To(this DateTime begin, DateTime end)
        {
            return new DateSpan(begin, end);
        }

        public static DateTime At(this DateTime dateTime, int hours, int minutes, int seconds)
        {
            return dateTime.At(new TimeSpan(hours, minutes, seconds));
        }

        public static DateTime At(this DateTime dateTime, TimeSpan time)
        {
            return dateTime.Date + time;
        }
    }
}
