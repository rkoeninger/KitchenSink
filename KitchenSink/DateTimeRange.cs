using System;
using System.Globalization;

namespace KitchenSink
{
    public struct DateTimeRange
    {
        public static DateTimeRange GetEntireDay(DateTime dt)
        {
            return GetEntireDay(dt.Year, dt.Month, dt.Day);
        }

        public static DateTimeRange GetEntireDay(int year, Month month, int day)
        {
            return GetEntireDay(year, (int)month, day);
        }

        public static DateTimeRange GetEntireDay(int year, int month, int day)
        {
            var begin = new DateTime(year, month, day);
            return new DateTimeRange(begin, begin.AddDays(1));
        }

        public static DateTimeRange GetEntireMonth(DateTime dt)
        {
            return GetEntireMonth(dt.Year, dt.Month);
        }

        public static DateTimeRange GetEntireMonth(int year, Month month)
        {
            return GetEntireMonth(year, (int)month);
        }

        public static DateTimeRange GetEntireMonth(int year, int month)
        {
            var begin = new DateTime(year, month, 1);
            return new DateTimeRange(begin, begin.AddMonths(1));
        }

        public static DateTimeRange GetEntireYear(DateTime dt)
        {
            return GetEntireYear(dt.Year);
        }

        public static DateTimeRange GetEntireYear(int year)
        {
            var begin = new DateTime(year, 1, 1);
            return new DateTimeRange(begin, begin.AddYears(1));
        }

        public static DateTimeRange Parse(string s, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var i = s.IndexOf(DateTimeSeparator, comparison);

            if (i < 0 || i > (s.Length - DateTimeSeparator.Length))
                throw new ArgumentException("Invalid DateTimeRange string");

            var beginString = s.Substring(0, i);
            var endString = s.Substring(i + DateTimeSeparator.Length);

            return new DateTimeRange(DateTime.Parse(beginString), DateTime.Parse(endString));
        }

        public static DateTimeRange ParseExact(string s, string format, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var i = s.IndexOf(DateTimeSeparator, comparison);

            if (i < 0 || i > (s.Length - DateTimeSeparator.Length))
                throw new ArgumentException("Invalid DateTimeRange string");

            var beginString = s.Substring(0, i);
            var endString = s.Substring(i + DateTimeSeparator.Length);

            return new DateTimeRange(
                DateTime.ParseExact(beginString, format, CultureInfo.InvariantCulture),
                DateTime.ParseExact(endString, format, CultureInfo.InvariantCulture));
        }

        public static readonly string DateTimeSeparator = " to ";

        public DateTimeRange(DateTime begin, DateTime end) : this()
        {
            if (end < begin)
                throw new ArgumentException($"{nameof(end)} can't come before {nameof(begin)}");

            Begin = begin;
            End = end;
        }

        public DateTimeRange(DateTime begin, TimeSpan length) : this()
        {
            if (length < TimeSpan.Zero)
                throw new ArgumentException($"{nameof(length)} can't be negative");

            Begin = begin;
            End = begin + length;
        }

        public DateTime Begin { get; }
        public DateTime End { get; }
        public TimeSpan Length => End - Begin;

        public bool Contains(DateTime time)
        {
            return time >= Begin && time < End;
        }

        public bool Contains(DateTimeRange that)
        {
            return that.Begin >= Begin && that.End <= End;
        }

        public bool Overlaps(DateTimeRange that)
        {
            return Contains(that)
                || (that.Begin < End && that.End > Begin)
                || (Begin < that.End && End > that.Begin);
        }

        public string ToString(string format)
        {
            return Begin.ToString(format) + DateTimeSeparator + End.ToString(format);
        }
    }
}
