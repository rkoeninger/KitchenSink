using System;
using System.Globalization;

namespace KitchenSink.Timekeeping
{
    /// <summary>
    /// Represents a region of time that covers an entire year
    /// from 00:00 of the 1st of January up to 00:00 of the 1st of January of the next year.
    /// </summary>
    public struct ExactYear : IEquatable<ExactYear>, IComparable<ExactYear>
    {
        public static ExactYear On(int year) => new ExactYear(year);
        public static ExactYear Containing(DateTime dateTime) => new ExactYear(dateTime.Year);
        public static ExactYear Parse(string s) => Containing(DateTime.Parse(s));

        public static ExactYear ParseExact(string s, string format) =>
            Containing(DateTime.ParseExact(s, format, CultureInfo.InvariantCulture));

        public ExactYear(int year)
        {
            Year = year;
        }

        public static ExactYear MinValue => Containing(DateTime.MinValue);
        public static ExactYear MaxValue => Containing(DateTime.MaxValue);

        public int Year { get; }

        public DateTime Begin => new DateTime(Year, 1, 1);
        public DateTime End => Begin.AddYears(1);

        public bool Contains(DateTime dateTime) => Begin <= dateTime && End > dateTime;
        public bool Contains(DateSpan dateSpan) => Begin <= dateSpan.Begin && End >= dateSpan.End;
        public bool Contains(ExactDay exactDay) => Year == exactDay.Year;
        public bool Contains(ExactMonth exactMonth) => Year == exactMonth.Year;

        public DateSpan ToDateSpan() => DateSpan.EntireYear(Year);

        public override int GetHashCode() => Begin.GetHashCode() ^ End.GetHashCode();
        public override bool Equals(object obj) => obj is ExactYear year && Equals(year);
        public bool Equals(ExactYear that) => Begin == that.Begin && End == that.End;

        public int CompareTo(ExactYear that) =>
            Math.Sign((Begin == that.Begin ? End - that.End : Begin - that.Begin).Ticks);

        public override string ToString() => ToString("yyyy-MM");
        public string ToString(string format) => new DateTime(Year, 1, 1).ToString(format);

        public static bool operator ==(ExactYear x, ExactYear y) => x.Equals(y);
        public static bool operator !=(ExactYear x, ExactYear y) => !x.Equals(y);
        public static bool operator <(ExactYear x, ExactYear y) => x.CompareTo(y) < 0;
        public static bool operator >(ExactYear x, ExactYear y) => x.CompareTo(y) > 0;
        public static bool operator <=(ExactYear x, ExactYear y) => x.CompareTo(y) <= 0;
        public static bool operator >=(ExactYear x, ExactYear y) => x.CompareTo(y) >= 0;
    }
}
