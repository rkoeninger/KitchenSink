using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
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
    }
}
