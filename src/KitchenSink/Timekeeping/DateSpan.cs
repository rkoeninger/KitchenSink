using System;
using System.Globalization;
using System.Linq;
using KitchenSink.Extensions;

namespace KitchenSink.Timekeeping
{
    /// <summary>
    /// Represents a region of time between two instants, each
    /// represented as a <see cref="DateTime"/>.
    /// </summary>
    public readonly struct DateSpan : IEquatable<DateSpan>, IComparable<DateSpan>
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

        /// <summary>
        /// Parses string representation of <see cref="DateSpan"/> with optional
        /// separator and <see cref="StringComparison"/> mode.
        /// </summary>
        public static DateSpan Parse(
            string s,
            string sep = DateTimeSeparator,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var i = s.IndexOf(sep, comparison);

            if (i < 0 || i > (s.Length - sep.Length))
            {
                throw new ArgumentException("Invalid DateTimeRange string");
            }

            var beginString = s[0..i];
            var endString = s[..(i + sep.Length)];
            return new DateSpan(DateTime.Parse(beginString), DateTime.Parse(endString));
        }

        /// <summary>
        /// Parses string representation of <see cref="DateSpan"/> with separator,
        /// exact <see cref="DateTime"/> format and optional
        /// <see cref="StringComparison"/> mode.
        /// </summary>
        public static DateSpan ParseExact(
            string s,
            string sep,
            string format,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var i = s.IndexOf(sep, comparison);

            if (i < 0 || i > (s.Length - sep.Length))
            {
                throw new ArgumentException("Invalid DateTimeRange string");
            }

            var beginString = s.Substring(0, i);
            var endString = s.Substring(i + sep.Length);
            return new DateSpan(
                DateTime.ParseExact(beginString, format, CultureInfo.InvariantCulture),
                DateTime.ParseExact(endString, format, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The character sequence that will separate the string represenations
        /// of two <see cref="DateTime"/>'s when a <see cref="DateSpan"/> is
        /// parsed or string'd.
        /// </summary>
        public const string DateTimeSeparator = " to ";

        /// <summary>
        /// Creates a <see cref="DateSpan"/> between the given <see cref="DateTime"/>'s,
        /// swapping them if they are in the wrong order.
        /// </summary>
        public DateSpan(DateTime begin, DateTime end) : this()
        {
            Begin = begin.Min(end);
            End = begin.Max(end);
        }

        /// <summary>
        /// Creates a <see cref="DateSpan"/> from given instant with duration,
        /// Reversing the span if it would go backward.
        /// </summary>
        public DateSpan(DateTime begin, TimeSpan duration) : this()
        {
            var end = begin + duration;
            Begin = begin.Min(end);
            End = begin.Max(end);
        }

        /// <summary>
        /// A span from the earliest to the latest representable <see cref="DateTime"/>.
        /// </summary>
        public static DateSpan Forever => new DateSpan(DateTime.MinValue, DateTime.MaxValue);

        /// <summary>
        /// The earliest moment in this span, inclusive.
        /// </summary>
        public DateTime Begin { get; }

        /// <summary>
        /// The latest moment in this span, exclusive.
        /// </summary>
        public DateTime End { get; }

        /// <summary>
        /// The duration of this span.
        /// </summary>
        public TimeSpan Length => End - Begin;

        /// <summary>
        /// Returns true if this <see cref="DateSpan"/> contains the
        /// given instant <see cref="DateTime"/>.
        /// </summary>
        public bool Contains(DateTime time) => time >= Begin && time < End;

        /// <summary>
        /// Returns true if this <see cref="DateSpan"/> completely
        /// contains the given <see cref="DateSpan"/>.
        /// </summary>
        public bool Contains(DateSpan that) => that.Begin >= Begin && that.End <= End;

        /// <summary>
        /// Returns true if this <see cref="DateSpan"/> contains any
        /// <see cref="DateTime"/>'s also contained by the given <see cref="DateSpan"/>.
        /// </summary>
        public bool Overlaps(DateSpan that) =>
            Contains(that)
                || that.Begin < End && that.End > Begin
                || Begin < that.End && End > that.Begin;

        /// <summary>
        /// Computes hash code using XOR of begin and end.
        /// </summary>
        public override int GetHashCode() => Begin.GetHashCode() ^ End.GetHashCode();

        /// <summary>
        /// Argument must be a span and they must have the same beginning and ending.
        /// </summary>
        public override bool Equals(object obj) => obj is DateSpan span && Equals(span);

        /// <summary>
        /// Two spans are equal if they have the same beginning and ending.
        /// </summary>
        public bool Equals(DateSpan that) => Begin == that.Begin && End == that.End;

        /// <summary>
        /// Compares two spans for total ordering.
        /// </summary>
        public int CompareTo(DateSpan that) =>
            Math.Sign((Begin == that.Begin ? End - that.End : Begin - that.Begin).Ticks);

        /// <summary>
        /// Renders string representation in default format.
        /// </summary>
        public override string ToString() =>
            ToString(CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern);

        /// <summary>
        /// Renders string representation in given format with optional begin-end separator.
        /// </summary>
        public string ToString(string format, string sep = DateTimeSeparator) =>
            $"{Begin.ToString(format)}{sep}{End.ToString(format)}";

        public static bool operator ==(DateSpan x, DateSpan y) => x.Equals(y);
        public static bool operator !=(DateSpan x, DateSpan y) => !x.Equals(y);
        public static bool operator <(DateSpan x, DateSpan y) => x.CompareTo(y) < 0;
        public static bool operator >(DateSpan x, DateSpan y) => x.CompareTo(y) > 0;
        public static bool operator <=(DateSpan x, DateSpan y) => x.CompareTo(y) <= 0;
        public static bool operator >=(DateSpan x, DateSpan y) => x.CompareTo(y) >= 0;
    }
}
