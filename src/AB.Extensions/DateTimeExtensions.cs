using System;

namespace AB.Extensions
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime SQL_SMALLDATE_MIN = new DateTime(1900, 01, 01, 00, 00, 00);
        public static readonly DateTime SQL_SMALLDATE_MAX = new DateTime(2079, 06, 06, 23, 59, 00);
        public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0);

        public static bool IsBetween(this DateTime dt, DateTime startDate, DateTime endDate, bool compareTime = false)
        {
            return compareTime ?
               dt >= startDate && dt <= endDate :
               dt.Date >= startDate.Date && dt.Date <= endDate.Date;
        }

        public static bool IsLeapYear(this DateTime value)
        {
            return (DateTime.DaysInMonth(value.Year, 2) == 29);
        }

        /// <summary>
        /// Returns true if Saturday or Sunday.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsWeekend(this DayOfWeek d)
        {
            return !d.IsWeekday();
        }

        /// <summary>
        /// Returns true if we're on this Monday through Friday plane.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsWeekday(this DayOfWeek d)
        {
            switch (d)
            {
                case DayOfWeek.Sunday:
                case DayOfWeek.Saturday: return false;
                default: return true;
            }
        }

        public static DateTime AddWorkdays(this DateTime d, int days)
        {
            // start from a weekday
            while (d.DayOfWeek.IsWeekday()) d = d.AddDays(1.0);
            for (int i = 0; i < days; ++i)
            {
                d = d.AddDays(1.0);
                while (d.DayOfWeek.IsWeekday()) d = d.AddDays(1.0);
            }
            return d;
        }

        /// <summary>
        /// Converts a System.DateTime object to Unix timestamp
        /// </summary>
        /// <returns>The Unix timestamp</returns>
        public static long ToUnixTimestamp(this DateTime date)
        {
            TimeSpan unixTimeSpan = date - UNIX_EPOCH;
            return (long)unixTimeSpan.TotalSeconds;
        }
    }
}
