using System;

namespace ZedSharp
{
    public struct ZonedDateTime : IComparable<ZonedDateTime>, IEquatable<ZonedDateTime>
    {
        public ZonedDateTime(DateTime dateTime, TimeZoneInfo timeZoneInfo) : this()
        {
            DateTime = dateTime;
            TimeZoneInfo = timeZoneInfo;
        }

        public DateTime DateTime { get; private set; }
        public TimeZoneInfo TimeZoneInfo { get; private set; }

        public DateTime AsUtc()
        {
            return TimeZoneInfo.ConvertTimeToUtc(DateTime, TimeZoneInfo);
        }

        public static implicit operator DateTime(ZonedDateTime zonedDateTime)
        {
            return zonedDateTime.AsUtc();
        }

        public int CompareTo(ZonedDateTime that)
        {
            return AsUtc().CompareTo(that.AsUtc());
        }

        public static bool operator >(ZonedDateTime x, ZonedDateTime y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <(ZonedDateTime x, ZonedDateTime y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >=(ZonedDateTime x, ZonedDateTime y)
        {
            return x.CompareTo(y) >= 0;
        }

        public static bool operator <=(ZonedDateTime x, ZonedDateTime y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator ==(ZonedDateTime x, ZonedDateTime y)
        {
            return x.CompareTo(y) == 0;
        }

        public static bool operator !=(ZonedDateTime x, ZonedDateTime y)
        {
            return x.CompareTo(y) != 0;
        }

        public override bool Equals(Object obj)
        {
            return (obj is ZonedDateTime) && Equals((ZonedDateTime) obj);
        }

        public bool Equals(ZonedDateTime that)
        {
            return DateTime == that.DateTime & Equals(TimeZoneInfo, that.TimeZoneInfo);
        }

        public override int GetHashCode()
        {
            return DateTime.GetHashCode() ^ (TimeZoneInfo == null ? 0 : TimeZoneInfo.GetHashCode());
        }
    }
}
