using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
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

        public static DateTimeRange Parse(String s)
        {
            int i = s.IndexOf(DateTimeSeparator);

            if (i < 0 || i > (s.Length - DateTimeSeparator.Length))
                throw new ArgumentException("Invalid DateTimeRange string");

            var beginString = s.Substring(0, i);
            var endString = s.Substring(i + DateTimeSeparator.Length);

            return new DateTimeRange(DateTime.Parse(beginString), DateTime.Parse(endString));
        }

        public static DateTimeRange ParseExact(String s, String format)
        {
            int i = s.IndexOf(DateTimeSeparator);

            if (i < 0 || i > (s.Length - DateTimeSeparator.Length))
                throw new ArgumentException("Invalid DateTimeRange string");

            var beginString = s.Substring(0, i);
            var endString = s.Substring(i + DateTimeSeparator.Length);

            return new DateTimeRange(
                DateTime.ParseExact(beginString, format, CultureInfo.InvariantCulture),
                DateTime.ParseExact(endString, format, CultureInfo.InvariantCulture));
        }

        public static readonly String DateTimeSeparator = " to ";

        public DateTimeRange(DateTime begin, DateTime end) : this()
        {
            if (end < begin)
                throw new ArgumentException("End can't be earlier than Begin");

            Begin = begin;
            End = end;
            Length = end - begin;
        }

        public DateTimeRange(DateTime begin, TimeSpan length) : this()
        {
            if (length < TimeSpan.Zero)
                throw new ArgumentException("Length can't be negative");

            Begin = begin;
            End = begin + length;
            Length = length;
        }

        public DateTime Begin { get; private set; }
        public DateTime End { get; private set; }
        public TimeSpan Length { get; private set; }

        public bool Contains(DateTime time)
        {
            return time >= Begin && time < End;
        }

        public bool Contains(DateTimeRange that)
        {
            return that.Begin >= this.Begin && that.End <= this.End;
        }

        public bool Overlaps(DateTimeRange that)
        {
            return Contains(that)
                || (that.Begin < this.End && that.End > this.Begin)
                || (this.Begin < that.End && this.End > that.Begin);
        }

        public String ToString(String format)
        {
            return Begin.ToString(format) + DateTimeSeparator + End.ToString(format);
        }
    }
}
