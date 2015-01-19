using System;

namespace ZedSharp
{
    public enum Month
    {
        January = 1, February, March, April, May, June, July, August, September, October, November, December
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

        public static DateTime Jan(this int year, int day)
        {
            return new DateTime(year, 1, day);
        }

        public static DateTime Feb(this int year, int day)
        {
            return new DateTime(year, 2, day);
        }

        public static DateTime Mar(this int year, int day)
        {
            return new DateTime(year, 3, day);
        }

        public static DateTime Apr(this int year, int day)
        {
            return new DateTime(year, 4, day);
        }

        public static DateTime May(this int year, int day)
        {
            return new DateTime(year, 5, day);
        }

        public static DateTime Jun(this int year, int day)
        {
            return new DateTime(year, 6, day);
        }

        public static DateTime Jul(this int year, int day)
        {
            return new DateTime(year, 7, day);
        }

        public static DateTime Aug(this int year, int day)
        {
            return new DateTime(year, 8, day);
        }

        public static DateTime Sep(this int year, int day)
        {
            return new DateTime(year, 9, day);
        }

        public static DateTime Oct(this int year, int day)
        {
            return new DateTime(year, 10, day);
        }

        public static DateTime Nov(this int year, int day)
        {
            return new DateTime(year, 11, day);
        }

        public static DateTime Dec(this int year, int day)
        {
            return new DateTime(year, 12, day);
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
