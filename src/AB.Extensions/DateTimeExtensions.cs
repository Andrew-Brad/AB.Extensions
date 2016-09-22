using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB.Extensions
{
    public static class DateTimeExtensions
    {
        public static readonly DateTime SQL_SMALLDATE_MIN = new DateTime(1900, 01, 01, 00, 00, 00);
        public static readonly DateTime SQL_SMALLDATE_MAX = new DateTime(2079, 06, 06, 23, 59, 00);

        public static DateTime GetSQLSmallDateMax(this DateTime dt)
        {
            return DateTimeExtensions.SQL_SMALLDATE_MAX;
        }

        public static DateTime GetSQLSmallDateMin(this DateTime dt)
        {
            return DateTimeExtensions.SQL_SMALLDATE_MIN;
        }
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
    }
}
