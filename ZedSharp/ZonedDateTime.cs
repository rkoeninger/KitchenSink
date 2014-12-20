using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public struct ZonedDateTime : IComparable<ZonedDateTime>
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
    }
}
