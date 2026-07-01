using Xunit;

namespace AB.Extensions.Tests;

public class IntExtensionsTests
{
    // LeadingDigit returns the most significant digit of the number's magnitude (sign ignored),
    // or 0 for zero. The tricky cases are the sign fold and the int.MinValue negation, which can't
    // be represented as a positive int — the implementation widens to long to avoid overflow.
    [Theory]
    // value          expected  note
    [InlineData(0, 0)]              // zero is the only input that yields 0
    [InlineData(1, 1)]
    [InlineData(9, 9)]
    [InlineData(10, 1)]            // digit count rolls over
    [InlineData(42, 4)]
    [InlineData(100, 1)]
    [InlineData(987654321, 9)]
    [InlineData(1000000000, 1)]    // ten digits, the largest power of ten in int range
    [InlineData(-1, 1)]            // sign ignored
    [InlineData(-10, 1)]
    [InlineData(-42, 4)]           // -42 folds to the same digit as 42
    [InlineData(int.MaxValue, 2)]  // 2147483647
    [InlineData(int.MinValue, 2)]  // -2147483648: negation would overflow int, hence the long widen
    public void LeadingDigit_ReturnsMostSignificantDigit_IgnoringSign(int value, int expected) =>
        Assert.Equal(expected, value.LeadingDigit());
}
