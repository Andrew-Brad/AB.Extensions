namespace AB.Extensions;

/// <summary>
/// Extensions for the <see cref="Int32"/> class.
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// Returns the leading (most significant) decimal digit of an integer, ignoring its sign.
    /// </summary>
    /// <param name="value">The number whose leading digit is wanted.</param>
    /// <returns>
    /// The first digit of the number's magnitude (1–9), or <c>0</c> when <paramref name="value"/> is zero.
    /// The sign is ignored, so <c>-42</c> and <c>42</c> both yield <c>4</c>.
    /// </returns>
    public static int LeadingDigit(this int value)
    {
        // Widen to long before negating so int.MinValue (which has no positive int counterpart) doesn't overflow.
        uint magnitude = value < 0 ? (uint)-(long)value : (uint)value;
        while (magnitude >= 10) magnitude /= 10;
        return (int)magnitude;
    }
}
