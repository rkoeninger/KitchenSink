using System;
using System.Globalization;

namespace KitchenSink.Timekeeping
{
    /// <summary>
    /// Represents a region of time between two instants, each
    /// represented as a <see cref="DateTime"/>.
    /// </summary>
    public struct DateSpan : IEquatable<DateSpan>, IComparable<DateSpan>
    {
        /// <summary>
        /// Returns <see cref="DateSpan"/> of entire day containing
        /// given <see cref="DateTime"/> starting at 00:00 that morning.
        /// </summary>
        public static DateSpan EntireDay(DateTime dt) => EntireDay(dt.Year, dt.Month, dt.Day);

        /// <summary>
        /// Returns <see cref="DateSpan"/> of entire day containing
        /// given <see cref="DateTime"/> starting at 00:00 that morning.
        /// </summary>
        public static DateSpan EntireDay(int year, int month, int day)
        {
            var begin = new DateTime(year, month, day);
            return new DateSpan(begin, begin.AddDays(1));
        }

        /// <summary>
        /// Returns <see cref="DateSpan"/> of entire week containing
        /// given <see cref="DateTime"/> starting on Sunday.
        /// </summary>
        public static DateSpan EntireWeek(DateTime dt)
        {
            var diff = (int)dt.DayOfWeek;
            var startingDay = new DateTime(dt.Year, dt.Month, dt.Day).AddDays(-diff);
            return new DateSpan(startingDay, TimeSpan.FromDays(7));
        }
        
        /// <summary>
        /// Returns <see cref="DateSpan"/> of entire week containing
        /// given <see cref="DateTime"/> starting on Sunday.
        /// </summary>
        public static DateSpan EntireWeek(int year, int month, int day) => EntireWeek(new DateTime(year, month, day));

        /// <summary>
        /// Returns <see cref="DateSpan"/> of entire month containing
        /// given <see cref="DateTime"/> starting on the 1st.
        /// </summary>
        public static DateSpan EntireMonth(DateTime dt) => EntireMonth(dt.Year, dt.Month);

        /// <summary>
        /// Returns <see cref="DateSpan"/> of entire month containing
        /// given <see cref="DateTime"/> starting on the 1st.
        /// </summary>
        public static DateSpan EntireMonth(int year, int month)
        {
            var begin = new DateTime(year, month, 1);
            return new DateSpan(begin, begin.AddMonths(1));
        }
        
        /// <summary>
        /// Returns <see cref="DateSpan"/> of entire year containing
        /// given <see cref="DateTime"/> starting on the 1st of January.
        /// </summary>
        public static DateSpan EntireYear(DateTime dt) => EntireYear(dt.Year);

        /// <summary>
        /// Returns <see cref="DateSpan"/> of entire year containing
        /// given <see cref="DateTime"/> starting on the 1st of January.
        /// </summary>
        public static DateSpan EntireYear(int year)
        {
            var begin = new DateTime(year, 1, 1);
            return new DateSpan(begin, begin.AddYears(1));
        }

        public static DateSpan Parse(string s, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var i = s.IndexOf(DateTimeSeparator, comparison);

            if (i < 0 || i > (s.Length - DateTimeSeparator.Length))
                throw new ArgumentException("Invalid DateTimeRange string");

            var beginString = s.Substring(0, i);
            var endString = s.Substring(i + DateTimeSeparator.Length);

            return new DateSpan(DateTime.Parse(beginString), DateTime.Parse(endString));
        }

        public static DateSpan ParseExact(string s, string format, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var i = s.IndexOf(DateTimeSeparator, comparison);

            if (i < 0 || i > (s.Length - DateTimeSeparator.Length))
                throw new ArgumentException("Invalid DateTimeRange string");

            var beginString = s.Substring(0, i);
            var endString = s.Substring(i + DateTimeSeparator.Length);

            return new DateSpan(
                DateTime.ParseExact(beginString, format, CultureInfo.InvariantCulture),
                DateTime.ParseExact(endString, format, CultureInfo.InvariantCulture));
        }

        public const string DateTimeSeparator = " to ";

        public DateSpan(DateTime begin, DateTime end) : this()
        {
            if (end < begin)
                throw new ArgumentException($"{nameof(end)} can't come before {nameof(begin)}");

            Begin = begin;
            End = end;
        }

        public DateSpan(DateTime begin, TimeSpan length) : this()
        {
            if (length < TimeSpan.Zero)
                throw new ArgumentException($"{nameof(length)} can't be negative");

            Begin = begin;
            End = begin + length;
        }

        public static DateSpan MinValue => new DateSpan(DateTime.MinValue, DateTime.MinValue);
        public static DateSpan MaxValue => new DateSpan(DateTime.MinValue, DateTime.MaxValue);

        public DateTime Begin { get; }
        public DateTime End { get; }
        public TimeSpan Length => End - Begin;

        public bool Contains(DateTime time) => time >= Begin && time < End;

        public bool Contains(DateSpan that) => that.Begin >= Begin && that.End <= End;

        public bool Overlaps(DateSpan that) =>
            Contains(that)
                || that.Begin < End && that.End > Begin
                || Begin < that.End && End > that.Begin;

        public override int GetHashCode() => Begin.GetHashCode() ^ End.GetHashCode();

        public override bool Equals(object obj) => obj is DateSpan && Equals((DateSpan)obj);

        public bool Equals(DateSpan that) => Begin == that.Begin && End == that.End;

        public int CompareTo(DateSpan that) =>
            Math.Sign((Begin == that.Begin ? End - that.End : Begin - that.Begin).Ticks);

        public override string ToString() =>
            ToString(CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern);

        public string ToString(string format) => $"{Begin:format} to {End:format}";

        public static bool operator ==(DateSpan x, DateSpan y) => x.Equals(y);

        public static bool operator !=(DateSpan x, DateSpan y) => !x.Equals(y);

        public static bool operator <(DateSpan x, DateSpan y) => x.CompareTo(y) < 0;

        public static bool operator >(DateSpan x, DateSpan y) => x.CompareTo(y) > 0;

        public static bool operator <=(DateSpan x, DateSpan y) => x.CompareTo(y) <= 0;

        public static bool operator >=(DateSpan x, DateSpan y) => x.CompareTo(y) >= 0;
    }
}
