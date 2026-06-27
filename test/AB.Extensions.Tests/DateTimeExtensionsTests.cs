using System;
using System.Globalization;

using AB.Extensions;

using Xunit;

namespace ABExtensions.Tests;

public class DateTimeExtensionsTests
{
    [Fact]
    public void IsBetween_1() => Assert.True(DateTime.Now.IsBetween(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1)));

    [Fact]
    public void IsBetween_2()
    {
        bool isBetween = DateTime.Now.IsBetween(DateTime.Now.AddSeconds(1), DateTime.Now.AddSeconds(2), true);
        Assert.False(isBetween);
    }

    [Fact]
    public void IsBetween_3() => Assert.False(DateTime.Now.IsBetween(DateTime.Now.AddSeconds(1), DateTime.Now.AddSeconds(1), true));

    [Fact]
    public void IsBetween_4() => Assert.True(DateTime.MinValue.IsBetween(DateTime.MinValue, DateTime.MinValue, true)); //identical dates are considered between each other

    // Flipped bounds: startDate > endDate should produce the same result as the correct order.
    [Fact]
    public void IsBetween_FlippedBounds_DateInRange_ReturnsTrue() =>
        Assert.True(new DateTime(2024, 6, 15).IsBetween(new DateTime(2024, 12, 31), new DateTime(2024, 1, 1)));

    [Fact]
    public void IsBetween_FlippedBounds_DateOutOfRange_ReturnsFalse() =>
        Assert.False(new DateTime(2025, 1, 1).IsBetween(new DateTime(2024, 12, 31), new DateTime(2024, 1, 1)));

    [Fact]
    public void IsBetween_FlippedBounds_WithTime_DateInRange_ReturnsTrue() =>
        Assert.True(new DateTime(2024, 6, 15, 12, 0, 0).IsBetween(new DateTime(2024, 12, 31), new DateTime(2024, 1, 1), compareTime: true));

    [Fact]
    public void Year_2000_Is_LeapYear() => Assert.True(DateTime.IsLeapYear(2000));

    [Fact]
    public void Year_1921_Is_LeapYear() => Assert.False(DateTime.IsLeapYear(1921));

    [Fact]
    public void Year_1900_Is_LeapYear() => Assert.False(DateTime.IsLeapYear(1900));

    [Fact]
    public void LeapYears_Negative_Cases()
    {
        Assert.False(DateTime.IsLeapYear(2019));
        Assert.True(DateTime.IsLeapYear(2020));
        Assert.False(DateTime.IsLeapYear(2021));
    }

    // --- AddWorkdays: anchored on the week of Mon 2026-06-01 ... Sun 2026-06-07 ---
    // Contract: a weekend start normalizes forward to Monday, then each workday step skips weekends.

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

    // Negative steps walk backwards. A weekend start normalizes *backwards* to Friday
    // (the direction of travel), the mirror of the forward-to-Monday positive case.

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

    // Round-trip: from a weekday start, +n then -n returns to the start (and vice versa).
    [Theory]
    [InlineData("2026-06-01")] // Mon
    [InlineData("2026-06-03")] // Wed
    [InlineData("2026-06-05")] // Fri
    public void AddWorkdays_NegativeReversesPositive_ForWeekdayStarts(string start)
    {
        foreach (int days in new[] { 1, 3, 5, 10 })
        {
            DateTime forward = Date(start).AddWorkdays(days);
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
            DateTime result = Date(start).AddWorkdays(days);
            Assert.True(result.DayOfWeek.IsWeekday(), $"start {start} +{days} landed on {result.DayOfWeek} ({result:yyyy-MM-dd})");
        }
    }

    private static DateTime Date(string iso) => DateTime.ParseExact(iso, "yyyy-MM-dd", CultureInfo.InvariantCulture);
}
