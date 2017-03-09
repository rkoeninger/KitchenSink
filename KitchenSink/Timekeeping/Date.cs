using System;

namespace KitchenSink.Timekeeping
{
    public static class Date
    {
        public static DateTime On(int year, int month, int day)
        {
            return new DateTime(year, month, day);
        }
    }
}
