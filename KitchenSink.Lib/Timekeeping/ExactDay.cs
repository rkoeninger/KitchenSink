using System;
using System.Globalization;

namespace KitchenSink.Timekeeping
{
    /// <summary>
    /// Represents a region of time that covers an entire day
    /// from midnight to midnight.
    /// </summary>
    public struct ExactDay : IEquatable<ExactDay>, IComparable<ExactDay>
    {
        public static ExactDay Containing(DateTime dateTime) =>
            new ExactDay(dateTime.Year, dateTime.Month, dateTime.Day);

        public static ExactDay Parse(string s) => Containing(DateTime.Parse(s));

        public static ExactDay ParseExact(string s, string format) =>
            Containing(DateTime.ParseExact(s, format, CultureInfo.InvariantCulture));

        public static ExactDay On(int year, int month, int day) => new ExactDay(year, month, day);

        public ExactDay(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public static ExactDay MinValue => Containing(DateTime.MinValue);
        public static ExactDay MaxValue => Containing(DateTime.MaxValue);

        public int Year { get; }
        public int Month { get; }
        public int Day { get; }

        public DateTime Begin => new DateTime(Year, Month, Day);
        public DateTime End => Begin.AddDays(1);

        public bool Contains(DateTime dateTime) => Begin <= dateTime && End > dateTime;

        public bool Contains(DateSpan dateSpan) => Begin <= dateSpan.Begin && End >= dateSpan.End;

        public override int GetHashCode() => Begin.GetHashCode() ^ End.GetHashCode();

        public override bool Equals(object obj) => obj is ExactDay && Equals((ExactDay)obj);

        public bool Equals(ExactDay that) => Begin == that.Begin && End == that.End;

        public int CompareTo(ExactDay that) =>
            Math.Sign((Begin == that.Begin ? End - that.End : Begin - that.Begin).Ticks);

        public override string ToString() => ToString("yyyy-MM-dd");

        public string ToString(string format) => new DateTime(Year, Month, Day).ToString(format);

        public static bool operator ==(ExactDay x, ExactDay y) => x.Equals(y);

        public static bool operator !=(ExactDay x, ExactDay y) => !x.Equals(y);

        public static bool operator <(ExactDay x, ExactDay y) => x.CompareTo(y) < 0;

        public static bool operator >(ExactDay x, ExactDay y) => x.CompareTo(y) > 0;

        public static bool operator <=(ExactDay x, ExactDay y) => x.CompareTo(y) <= 0;

        public static bool operator >=(ExactDay x, ExactDay y) => x.CompareTo(y) >= 0;
    }
}
