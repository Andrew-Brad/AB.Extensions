using Xunit;

namespace AB.Extensions.Tests;

public class IsNullTests
{
    // --- Reference-type overload: IsNull<T>() where T : class ---

    [Fact]
    public void IsNull_NullString_ReturnsTrue() => Assert.True(((string?)null).IsNull());

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("text")]
    public void IsNull_NonNullString_ReturnsFalse(string value) => Assert.False(value.IsNull());

    [Fact]
    public void IsNull_NullObject_ReturnsTrue() => Assert.True(((object?)null).IsNull());

    [Fact]
    public void IsNull_NonNullObject_ReturnsFalse() => Assert.False(new object().IsNull());

    // The check must use reference identity (is null), not a user-defined operator ==.
    [Fact]
    public void IsNull_TypeWithLyingEqualityOperator_UsesReferenceIdentity()
    {
        var instance = new OverloadedEquality();

        Assert.False(instance.IsNull());                    // not null by identity, though == always returns true
        Assert.True(((OverloadedEquality?)null).IsNull());  // genuinely null
    }

    // --- Value-type overload: IsNull<T>() where T : struct (operates on Nullable<T>) ---

    [Fact]
    public void IsNull_NullNullableInt_ReturnsTrue() => Assert.True(((int?)null).IsNull());

    [Theory]
    [InlineData(0)]     // zero is a value, not null — guards a HasValue vs default-value mix-up
    [InlineData(-1)]
    [InlineData(42)]
    public void IsNull_NullableIntWithValue_ReturnsFalse(int value) => Assert.False(((int?)value).IsNull());

    [Fact]
    public void IsNull_NullableBoolFalse_ReturnsFalse() => Assert.False(((bool?)false).IsNull());

    [Fact]
    public void IsNull_NullableEnum_ReflectsHasValue()
    {
        Assert.True(((System.DayOfWeek?)null).IsNull());
        Assert.False(((System.DayOfWeek?)System.DayOfWeek.Monday).IsNull());
    }

    // --- Predicate composition: the documented value — passable as Func<T,bool> via a method group,
    //     which the `is null` operator can't be. Overload resolution picks class vs struct by constraint. ---

    [Fact]
    public void IsNull_UsableAsPredicate_ForReferenceTypes()
    {
        string?[] values = { "a", null, "b", null };

        Assert.Equal(2, values.Count(StringExtensions.IsNull));
    }

    [Fact]
    public void IsNull_UsableAsPredicate_ForNullableValueTypes()
    {
        int?[] numbers = { 1, null, 3, null, null };

        Assert.Equal(3, numbers.Count(StringExtensions.IsNull));
    }

    // Equality operator deliberately lies, to prove IsNull relies on identity rather than ==.
    private sealed class OverloadedEquality
    {
        public static bool operator ==(OverloadedEquality? left, OverloadedEquality? right) => true;
        public static bool operator !=(OverloadedEquality? left, OverloadedEquality? right) => false;
        public override bool Equals(object? obj) => true;
        public override int GetHashCode() => 0;
    }
}
