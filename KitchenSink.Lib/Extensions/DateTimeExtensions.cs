using System;
using KitchenSink.Timekeeping;

namespace KitchenSink.Extensions
{
    public static class DateTimeExtensions
    {
        public static TimeSpan Days(this int x) => new TimeSpan(x, 0, 0, 0);

        public static TimeSpan Hours(this int x) => new TimeSpan(x, 0, 0);

        public static TimeSpan Minutes(this int x) => new TimeSpan(0, x, 0);

        public static TimeSpan Seconds(this int x) => new TimeSpan(0, 0, x);

        public static DateTime AgoLocal(this TimeSpan x) => DateTime.Now.Add(x.Negate());

        public static DateTime AgoUtc(this TimeSpan x) => DateTime.UtcNow.Add(x.Negate());

        public static DateTime AheadLocal(this TimeSpan x) => DateTime.Now.Add(x);

        public static DateTime AheadUtc(this TimeSpan x) => DateTime.UtcNow.Add(x);

        public static DateSpan To(this DateTime begin, DateTime end) => new DateSpan(begin, end);

        public static DateTime At(this DateTime dateTime, int hours, int minutes, int seconds) =>
            dateTime.At(new TimeSpan(hours, minutes, seconds));

        public static DateTime At(this DateTime dateTime, TimeSpan time) => dateTime.Date + time;
    }
}
