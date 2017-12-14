using System;

namespace KitchenSink.Timekeeping
{
    public struct ExactDay
    {
        public int Year { get; }
        public int Month { get; }
        public int Day { get; }

        public DateTime Start => new DateTime(Year, Month, Day);
        public DateTime End => Start.AddDays(1);

        public ExactDay(int year, int month, int day)
        {
            Year = year;
            Month = month;
            Day = day;
        }

        public bool Contains(DateTime dateTime)
        {
            return Start <= dateTime && End > dateTime;
        }

        public bool Contains(ExactDay day)
        {
            return Start <= day.Start && End >= day.End;
        }
    }
}
