using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZedSharp
{
    public abstract class Clock
    {
        public abstract DateTime Now { get; }
        
        private static readonly Clock LiveInstance = new LiveClock(TimeZoneInfo.Utc);

        public static Clock Live()
        {
            return LiveInstance;
        }

        public static Clock Live(TimeZoneInfo timeZone)
        {
            return new LiveClock(timeZone);
        }

        public static Clock WithOffset(TimeSpan offset)
        {
            return new OffsetClock(offset);
        }

        public static Clock StartingAt(DateTime offsetFrom)
        {
            return new OffsetClock(offsetFrom.ToUniversalTime() - DateTime.UtcNow);
        }

        public static Clock StoppedAt(DateTime fixedTime)
        {
            return new StoppedClock(fixedTime);
        }

        private class LiveClock : Clock
        {
            public LiveClock(TimeZoneInfo timeZone)
            {
                TimeZone = timeZone;
            }

            private readonly TimeZoneInfo TimeZone;

            public override DateTime Now
            {
                get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZone); }
            }
        }

        private class OffsetClock : Clock
        {
            public OffsetClock(TimeSpan offset)
            {
                Offset = offset;
            }

            private readonly TimeSpan Offset;

            public override DateTime Now
            {
                get { return DateTime.UtcNow + Offset; }
            }
        }

        private class StoppedClock : Clock
        {
            public StoppedClock(DateTime fixedTime)
            {
                FixedTime = fixedTime;
            }

            private readonly DateTime FixedTime;

            public override DateTime Now
            {
                get { return FixedTime; }
            }
        }
    }
}
