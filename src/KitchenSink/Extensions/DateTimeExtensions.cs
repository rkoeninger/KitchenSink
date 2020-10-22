using System;

namespace KitchenSink.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Creates the DateTime the given time before now in local time.
        /// </summary>
        public static DateTime AgoLocal(this TimeSpan x) => DateTime.Now.Subtract(x);

        /// <summary>
        /// Creates the DateTime the given time before now in universal time.
        /// </summary>
        public static DateTime AgoUtc(this TimeSpan x) => DateTime.UtcNow.Subtract(x);

        /// <summary>
        /// Creates the DateTime the given time ahead of now in local time.
        /// </summary>
        public static DateTime AheadLocal(this TimeSpan x) => DateTime.Now.Add(x);

        /// <summary>
        /// Creates the DateTime the given time ahead of now in universal time.
        /// </summary>
        public static DateTime AheadUtc(this TimeSpan x) => DateTime.UtcNow.Add(x);

        /// <summary>
        /// Creates a DateSpan between this and another DateTime.
        /// </summary>
        public static DateSpan To(this DateTime begin, DateTime end) => new DateSpan(begin, end);

        /// <summary>
        /// Creates a DateTime with its time replaced with the given time.
        /// </summary>
        public static DateTime At(this DateTime dateTime, TimeSpan time) => dateTime.Date + time;

        /// <summary>
        /// Creates a DateTime with its time replaced at the given hour.
        /// </summary>
        public static DateTime At(this DateTime dateTime, int hour) =>
            At(dateTime, new TimeSpan(hour, 0, 0));

        /// <summary>
        /// Creates a DateTime with its time replaced at the given hour:minute.
        /// </summary>
        public static DateTime At(this DateTime dateTime, int hour, int minute) =>
            At(dateTime, new TimeSpan(hour, minute, 0));

        /// <summary>
        /// Creates a DateTime with its time replaced at the given hour:minute:second.
        /// </summary>
        public static DateTime At(this DateTime dateTime, int hour, int minute, int second) =>
            At(dateTime, new TimeSpan(hour, minute, second));
    }
}
