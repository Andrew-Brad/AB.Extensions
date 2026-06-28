namespace AB.Extensions;

/// <summary>
/// Extension methods and constants for working with <see cref="DateTime"/> — range checks, weekday/weekend logic, and workday arithmetic.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>The minimum value representable by the SQL Server <c>smalldatetime</c> type (1900-01-01).</summary>
    public static readonly DateTime SqlSmallDateMin = new(1900, 01, 01, 00, 00, 00);

    /// <summary>The maximum value representable by the SQL Server <c>smalldatetime</c> type (2079-06-06 23:59).</summary>
    public static readonly DateTime SqlSmallDateMax = new(2079, 06, 06, 23, 59, 00);

    /// <summary>
    /// Returns true if <paramref name="thisDateTime"/> falls within [<paramref name="startDate"/>, <paramref name="endDate"/>], inclusive.
    /// The order of <paramref name="startDate"/> and <paramref name="endDate"/> does not matter; swapped bounds are normalized automatically.
    /// </summary>
    /// <param name="thisDateTime">The date to test.</param>
    /// <param name="startDate">One end of the range (inclusive).</param>
    /// <param name="endDate">The other end of the range (inclusive).</param>
    /// <param name="compareTime">When true, compares the full <see cref="DateTime"/> value; when false (default), compares date only.</param>
    public static bool IsBetween(this DateTime thisDateTime, DateTime startDate, DateTime endDate, bool compareTime = false)
    {
        if (startDate > endDate) (startDate, endDate) = (endDate, startDate);
        return compareTime ?
           thisDateTime >= startDate && thisDateTime <= endDate :
           thisDateTime.Date >= startDate.Date && thisDateTime.Date <= endDate.Date;
    }

    /// <summary>
    /// Returns true if the year of the given date is a leap year.
    /// </summary>
    /// <param name="value">The date whose year is tested.</param>
    /// <returns>True when February of that year has 29 days.</returns>
    public static bool IsLeapYear(this DateTime value) => (DateTime.DaysInMonth(value.Year, 2) == 29);

    /// <summary>
    /// Returns true if we're on this Monday through Friday plane.
    /// </summary>
    /// <param name="thisDayOfWeek"></param>
    /// <returns></returns>
    public static bool IsWeekday(this DayOfWeek thisDayOfWeek)
    {
        return thisDayOfWeek switch
        {
            DayOfWeek.Sunday or DayOfWeek.Saturday => false,
            _ => true
        };
    }

    /// <summary>
    /// Returns true if Saturday or Sunday.
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool IsWeekend(this DayOfWeek d) => !d.IsWeekday();

    // AddWorkdays is calendar-day arithmetic, so the modern surface uses DateOnly — no time-of-day, kind, or time zone to reason about.
    // netstandard2.0 has no DateOnly, so that single legacy TFM falls back to the DateTime form. The bodies are nearly identical.
#if NETSTANDARD2_0
    /// <summary>
    /// Adds the given number of workdays (Monday–Friday) to a date, skipping weekends.
    /// A negative <paramref name="days"/> walks backwards, subtracting workdays.
    /// </summary>
    /// <param name="d">The starting date.</param>
    /// <param name="days">The number of workdays to advance; negative values move backwards.</param>
    /// <returns>The resulting date, always landing on a weekday.</returns>
    public static DateTime AddWorkdays(this DateTime d, int days)
    {
        // A weekend start is normalized onto a weekday in the direction of travel (forward when days == 0, preserving the historic behaviour).
        int direction = Math.Sign(days);
        int step = direction == 0 ? 1 : direction;

        while (!d.DayOfWeek.IsWeekday()) d = d.AddDays(step);
        for (int i = 0; i < Math.Abs(days); ++i)
        {
            d = d.AddDays(direction);
            while (!d.DayOfWeek.IsWeekday()) d = d.AddDays(direction);
        }
        return d;
    }
#else
    /// <summary>
    /// Adds the given number of workdays (Monday–Friday) to a date, skipping weekends.
    /// A negative <paramref name="days"/> walks backwards, subtracting workdays.
    /// </summary>
    /// <param name="d">The starting date.</param>
    /// <param name="days">The number of workdays to advance; negative values move backwards.</param>
    /// <returns>The resulting date, always landing on a weekday.</returns>
    public static DateOnly AddWorkdays(this DateOnly d, int days)
    {
        // A weekend start is normalized onto a weekday in the direction of travel (forward when days == 0, preserving the historic behaviour).
        int direction = Math.Sign(days);
        int step = direction == 0 ? 1 : direction;

        while (!d.DayOfWeek.IsWeekday()) d = d.AddDays(step);
        for (int i = 0; i < Math.Abs(days); ++i)
        {
            d = d.AddDays(direction);
            while (!d.DayOfWeek.IsWeekday()) d = d.AddDays(direction);
        }
        return d;
    }
#endif
}
