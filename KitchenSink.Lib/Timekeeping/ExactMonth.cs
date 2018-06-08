using System;
using System.Globalization;

namespace KitchenSink.Timekeeping
{
    /// <summary>
    /// Represents a region of time that covers an entire month
    /// from 00:00 of the 1st up to 00:00 of the 1st of the next month.
    /// </summary>
    public struct ExactMonth : IEquatable<ExactMonth>, IComparable<ExactMonth>
    {
        public static ExactDay On(int year, int month, int day) => new ExactDay(year, month, day);

        public static ExactMonth Containing(DateTime dateTime) =>
            new ExactMonth(dateTime.Year, dateTime.Month);

        public static ExactMonth Parse(string s) => Containing(DateTime.Parse(s));

        public static ExactMonth ParseExact(string s, string format) =>
            Containing(DateTime.ParseExact(s, format, CultureInfo.InvariantCulture));

        public ExactMonth(int year, int month)
        {
            Year = year;
            Month = month;
        }

        public static ExactMonth MinValue => Containing(DateTime.MinValue);
        public static ExactMonth MaxValue => Containing(DateTime.MaxValue);

        public int Year { get; }
        public int Month { get; }

        public DateTime Begin => new DateTime(Year, Month, 1);
        public DateTime End => Begin.AddMonths(1);

        public bool Contains(DateTime dateTime) => Begin <= dateTime && End > dateTime;
        public bool Contains(DateSpan dateSpan) => Begin <= dateSpan.Begin && End >= dateSpan.End;
        public bool Contains(ExactDay exactDay) => Year == exactDay.Year && Month == exactDay.Month;

        public DateSpan ToDateSpan() => DateSpan.EntireMonth(Year, Month);

        public override int GetHashCode() => Begin.GetHashCode() ^ End.GetHashCode();
        public override bool Equals(object obj) => obj is ExactMonth month && Equals(month);
        public bool Equals(ExactMonth that) => Begin == that.Begin && End == that.End;

        public int CompareTo(ExactMonth that) =>
            Math.Sign((Begin == that.Begin ? End - that.End : Begin - that.Begin).Ticks);

        public override string ToString() => ToString("yyyy-MM");
        public string ToString(string format) => new DateTime(Year, Month, 1).ToString(format);

        public static bool operator ==(ExactMonth x, ExactMonth y) => x.Equals(y);
        public static bool operator !=(ExactMonth x, ExactMonth y) => !x.Equals(y);
        public static bool operator <(ExactMonth x, ExactMonth y) => x.CompareTo(y) < 0;
        public static bool operator >(ExactMonth x, ExactMonth y) => x.CompareTo(y) > 0;
        public static bool operator <=(ExactMonth x, ExactMonth y) => x.CompareTo(y) <= 0;
        public static bool operator >=(ExactMonth x, ExactMonth y) => x.CompareTo(y) >= 0;
    }
}
