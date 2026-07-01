using System.Globalization;
using Xunit;

namespace AB.Extensions.Tests;

public class DateTimeExtensionsTests
{
    // --- IsBetween ---
    // Bounds inclusive on both ends; reversed bounds normalized; default ignores time-of-day, compareTime opts in.

    [Theory]
    // thisDate      lowerBound    upperBound    expected
    [InlineData("2024-06-15", "2024-06-14", "2024-06-16", true)]  // squarely inside
    [InlineData("2024-06-14", "2024-06-14", "2024-06-16", true)]  // on the lower bound (inclusive)
    [InlineData("2024-06-16", "2024-06-14", "2024-06-16", true)]  // on the upper bound (inclusive)
    [InlineData("2024-06-15", "2024-06-15", "2024-06-15", true)]  // identical bounds: the date is "between" itself
    [InlineData("2024-06-13", "2024-06-14", "2024-06-16", false)] // just below the range
    [InlineData("2024-06-17", "2024-06-14", "2024-06-16", false)] // just above the range
    public void IsBetween_TreatsBoundsAsInclusive(string date, string lower, string upper, bool expected) =>
        Assert.Equal(expected, Dt(date).IsBetween(Dt(lower), Dt(upper)));

    // Reversed bounds must give the same answer as natural order.
    [Theory]
    [InlineData("2024-06-15", "2024-12-31", "2024-01-01", true)]  // inside the (flipped) range
    [InlineData("2025-01-01", "2024-12-31", "2024-01-01", false)] // outside the (flipped) range
    public void IsBetween_NormalizesReversedBounds(string date, string start, string end, bool expected) =>
        Assert.Equal(expected, Dt(date).IsBetween(Dt(start), Dt(end)));

    // compareTime flips the answer on identical inputs: off = date-only compare, on = full instant.
    [Theory]
    // thisInstant         lowerBound          upperBound          compareTime expected
    [InlineData("2024-06-15 08:00", "2024-06-15 09:00", "2024-06-15 17:00", false, true)]  // before the window, but same date
    [InlineData("2024-06-15 08:00", "2024-06-15 09:00", "2024-06-15 17:00", true, false)]  // before the window, time now matters
    [InlineData("2024-06-15 23:59", "2024-06-15 09:00", "2024-06-15 17:00", false, true)]  // after the window, but same date
    [InlineData("2024-06-15 12:00", "2024-06-15 09:00", "2024-06-15 17:00", true, true)]   // inside the time window
    [InlineData("2024-06-15 17:00", "2024-06-15 09:00", "2024-06-15 17:00", true, true)]   // on the upper bound, to the minute (inclusive)
    public void IsBetween_CompareTime_TogglesTimeOfDaySignificance(string instant, string lower, string upper, bool compareTime, bool expected) =>
        Assert.Equal(expected, Dt(instant).IsBetween(Dt(lower), Dt(upper), compareTime));

    // compareTime inclusivity holds down to a single tick. atUpper picks which bound the offset applies to.
    [Theory]
    // atUpper  tickOffset  expected
    [InlineData(false, 0L, true)]   // exactly on the lower bound (inclusive)
    [InlineData(false, -1L, false)] // one tick below the lower bound
    [InlineData(false, 1L, true)]   // one tick inside the lower bound
    [InlineData(true, 0L, true)]    // exactly on the upper bound (inclusive)
    [InlineData(true, 1L, false)]   // one tick above the upper bound
    [InlineData(true, -1L, true)]   // one tick inside the upper bound
    public void IsBetween_CompareTime_IsInclusiveToTheTick(bool atUpper, long tickOffset, bool expected)
    {
        DateTime lower = new(2024, 6, 15, 9, 0, 0);
        DateTime upper = new(2024, 6, 15, 17, 0, 0);
        DateTime point = (atUpper ? upper : lower).AddTicks(tickOffset);
        Assert.Equal(expected, point.IsBetween(lower, upper, compareTime: true));
    }

    // Degenerate window: bounds one tick apart. Both ends inclusive, a tick outside falls out.
    [Fact]
    public void IsBetween_CompareTime_HandlesAOneTickWideWindow()
    {
        DateTime lower = new(2024, 6, 15, 12, 0, 0);
        DateTime upper = lower.AddTicks(1);

        Assert.True(lower.IsBetween(lower, upper, compareTime: true));
        Assert.True(upper.IsBetween(lower, upper, compareTime: true));
        Assert.False(upper.AddTicks(1).IsBetween(lower, upper, compareTime: true));
    }

    // Pins the documented Kind-blindness: comparison is tick-only, so identical ticks with different
    // DateTimeKind values compare equal. A spec test that locks the behavior against a "helpful" refactor.
    [Fact]
    public void IsBetween_IgnoresDateTimeKind_ComparingByTicksAlone()
    {
        long ticks = new DateTime(2024, 6, 15, 12, 0, 0).Ticks;
        DateTime utc = new(ticks, DateTimeKind.Utc);
        DateTime local = new(ticks, DateTimeKind.Local);
        DateTime unspecified = new(ticks, DateTimeKind.Unspecified);

        // All three share the same ticks, so each sits "between" the others regardless of Kind.
        Assert.True(utc.IsBetween(local, unspecified, compareTime: true));
        Assert.True(local.IsBetween(utc, utc, compareTime: true));
    }

    // --- IsLeapYear ---
    // Gregorian rule: divisible by 4, EXCEPT centuries, EXCEPT centuries divisible by 400. One case per branch.

    [Theory]
    // year  expected  branch
    [InlineData(2024, true)]   // ÷4, not a century → leap
    [InlineData(2023, false)]  // not ÷4 → common
    [InlineData(2020, true)]   // ÷4 → leap
    [InlineData(2000, true)]   // century ÷400 → leap (the famous exception-to-the-exception)
    [InlineData(1900, false)]  // century not ÷400 → common
    [InlineData(2100, false)]  // century not ÷400 → common
    [InlineData(1600, true)]   // century ÷400 → leap
    [InlineData(1, false)]     // year 1 (DateTime.MinValue's year) → common
    public void IsLeapYear_FollowsTheGregorianRule(int year, bool expected) =>
        Assert.Equal(expected, new DateTime(year, 1, 1).IsLeapYear());

    // --- AddWorkdays: anchored on the week of Mon 2026-06-01 ... Sun 2026-06-07 ---
    // Exercises the DateOnly overload (net8+); a weekend start normalizes toward Monday, then steps skip weekends.

    [Theory]
    [InlineData("2026-06-01", "2026-06-02")] // Mon → Tue
    [InlineData("2026-06-02", "2026-06-03")] // Tue → Wed
    [InlineData("2026-06-03", "2026-06-04")] // Wed → Thu
    [InlineData("2026-06-04", "2026-06-05")] // Thu → Fri
    [InlineData("2026-06-05", "2026-06-08")] // Fri → next Mon (skips Sat/Sun)
    [InlineData("2026-06-06", "2026-06-09")] // Sat → normalize to Mon, then Tue
    [InlineData("2026-06-07", "2026-06-09")] // Sun → normalize to Mon, then Tue
    public void AddWorkdays_PlusOne_FromEachDayOfWeek(string start, string expected) =>
        Assert.Equal(Date(expected), Date(start).AddWorkdays(1));

    [Theory]
    [InlineData("2026-06-04", 2, "2026-06-08")]  // Thu +2 → Fri, then Mon
    [InlineData("2026-06-01", 5, "2026-06-08")]  // Mon +5 → next Mon
    [InlineData("2026-06-05", 5, "2026-06-12")]  // Fri +5 → next Fri
    [InlineData("2026-06-01", 10, "2026-06-15")] // Mon +10 → two weeks on (Mon)
    public void AddWorkdays_MultiDay_SkipsWeekends(string start, int days, string expected) =>
        Assert.Equal(Date(expected), Date(start).AddWorkdays(days));

    [Theory]
    [InlineData("2026-06-01", "2026-06-01")] // Mon +0 → same day
    [InlineData("2026-06-06", "2026-06-08")] // Sat +0 → normalize to Mon
    [InlineData("2026-06-07", "2026-06-08")] // Sun +0 → normalize to Mon
    public void AddWorkdays_Zero_NormalizesWeekendToNextWeekday(string start, string expected) =>
        Assert.Equal(Date(expected), Date(start).AddWorkdays(0));

    // Negative steps walk backwards; a weekend start normalizes back to Friday (mirror of the positive case).

    [Theory]
    [InlineData("2026-06-05", "2026-06-04")] // Fri → Thu
    [InlineData("2026-06-02", "2026-06-01")] // Tue → Mon
    [InlineData("2026-06-01", "2026-05-29")] // Mon → previous Fri (skips Sat/Sun)
    [InlineData("2026-06-06", "2026-06-04")] // Sat → normalize back to Fri, then Thu
    [InlineData("2026-06-07", "2026-06-04")] // Sun → normalize back to Fri, then Thu
    public void AddWorkdays_MinusOne_FromEachDayOfWeek(string start, string expected) =>
        Assert.Equal(Date(expected), Date(start).AddWorkdays(-1));

    [Theory]
    [InlineData("2026-06-08", 1, "2026-06-05")]  // Mon -1 → previous Fri
    [InlineData("2026-06-08", 5, "2026-06-01")]  // Mon -5 → previous Mon
    [InlineData("2026-06-05", 5, "2026-05-29")]  // Fri -5 → previous Fri
    [InlineData("2026-06-15", 10, "2026-06-01")] // Mon -10 → two weeks back (Mon)
    public void AddWorkdays_NegativeMultiDay_SkipsWeekends(string start, int days, string expected) =>
        Assert.Equal(Date(expected), Date(start).AddWorkdays(-days));

    // Round-trip: from a weekday start, +n then -n returns to the start.
    [Theory]
    [InlineData("2026-06-01")] // Mon
    [InlineData("2026-06-03")] // Wed
    [InlineData("2026-06-05")] // Fri
    public void AddWorkdays_NegativeReversesPositive_ForWeekdayStarts(string start)
    {
        foreach (int days in new[] { 1, 3, 5, 10 })
        {
            DateOnly forward = Date(start).AddWorkdays(days);
            Assert.Equal(Date(start), forward.AddWorkdays(-days));
        }
    }

    [Theory]
    [InlineData("2026-06-01")]
    [InlineData("2026-06-02")]
    [InlineData("2026-06-03")]
    [InlineData("2026-06-04")]
    [InlineData("2026-06-05")]
    [InlineData("2026-06-06")]
    [InlineData("2026-06-07")]
    public void AddWorkdays_ResultIsAlwaysAWeekday(string start)
    {
        foreach (int days in new[] { -10, -5, -1, 0, 1, 3, 5, 10 })
        {
            DateOnly result = Date(start).AddWorkdays(days);
            Assert.True(result.DayOfWeek.IsWeekday(), $"start {start} +{days} landed on {result.DayOfWeek} ({result:yyyy-MM-dd})");
        }
    }

    private static DateOnly Date(string iso) => DateOnly.ParseExact(iso, "yyyy-MM-dd", CultureInfo.InvariantCulture);

    // Parses either a bare date ("yyyy-MM-dd") or a date-with-time ("yyyy-MM-dd HH:mm").
    private static DateTime Dt(string value) =>
        DateTime.ParseExact(value, ["yyyy-MM-dd", "yyyy-MM-dd HH:mm"], CultureInfo.InvariantCulture, DateTimeStyles.None);
}
