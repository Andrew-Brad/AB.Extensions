using System;

namespace AB.Extensions
{
    /// <summary>
    /// Extension methods and constants for working with <see cref="DateTime"/> — range checks,
    /// weekday/weekend logic, workday arithmetic, and Unix time.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>The minimum value representable by the SQL Server <c>smalldatetime</c> type (1900-01-01).</summary>
        public static readonly DateTime SQL_SMALLDATE_MIN = new DateTime(1900, 01, 01, 00, 00, 00);
        /// <summary>The maximum value representable by the SQL Server <c>smalldatetime</c> type (2079-06-06 23:59).</summary>
        public static readonly DateTime SQL_SMALLDATE_MAX = new DateTime(2079, 06, 06, 23, 59, 00);

        /// <summary>
        /// Identical dates are considered between each other (inclusivity).
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="compareTime"></param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime dt, DateTime startDate, DateTime endDate, bool compareTime = false)
        {
            return compareTime ?
               dt >= startDate && dt <= endDate :
               dt.Date >= startDate.Date && dt.Date <= endDate.Date;
        }

        /// <summary>
        /// Returns true if the year of the given date is a leap year.
        /// </summary>
        /// <param name="value">The date whose year is tested.</param>
        /// <returns>True when February of that year has 29 days.</returns>
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

        /// <summary>
        /// Adds the given number of workdays (Monday–Friday) to a date, skipping weekends.
        /// </summary>
        /// <param name="d">The starting date.</param>
        /// <param name="days">The number of workdays to advance.</param>
        /// <returns>The resulting date, landing on a weekday.</returns>
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

        // Removed in v5.0.0: ToUnixTimestamp → new DateTimeOffset(date).ToUnixTimeSeconds()
        // (BCL, available on all targets, handles offset/UTC correctly).
    }
}
