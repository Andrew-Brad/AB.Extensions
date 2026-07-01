using Xunit;

namespace AB.Extensions.Tests;

public class IntExtensionsTests
{
    // Most significant digit of |value|; 0 for zero. Watch the sign fold and int.MinValue (can't negate in int).
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(9, 9)]
    [InlineData(10, 1)]
    [InlineData(42, 4)]
    [InlineData(100, 1)]
    [InlineData(987654321, 9)]
    [InlineData(1000000000, 1)]
    [InlineData(-1, 1)]
    [InlineData(-10, 1)]
    [InlineData(-42, 4)]            // sign ignored
    [InlineData(int.MaxValue, 2)]
    [InlineData(int.MinValue, 2)]  // negation overflows int → long widen
    public void LeadingDigit_ReturnsMostSignificantDigit_IgnoringSign(int value, int expected) =>
        Assert.Equal(expected, value.LeadingDigit());
}
