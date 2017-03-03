using System;

namespace KitchenSink
{
    // TODO: move into Operators/Extensions/(Calendar namespace)
    public enum Month
    {
        January = 1, February, March, April, May, June, July, August, September, October, November, December
    }

    public static class Date
    {
        public static DateTime On(int year, int month, int day)
        {
            return new DateTime(year, month, day);
        }
    }

    public static class Dates
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

        public static DateTime AgoLocal(this TimeSpan x)
        {
            return DateTime.Now.Add(x.Negate());
        }

        public static DateTime Ago(this TimeSpan x)
        {
            return DateTime.UtcNow.Add(x.Negate());
        }

        public static DateTime FromNowLocal(this TimeSpan x)
        {
            return DateTime.Now.Add(x);
        }

        public static DateTime FromNow(this TimeSpan x)
        {
            return DateTime.UtcNow.Add(x);
        }

        public static DateTimeRange To(this DateTime begin, DateTime end)
        {
            return new DateTimeRange(begin, end);
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
