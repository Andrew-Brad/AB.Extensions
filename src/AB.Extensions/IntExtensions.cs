using System;

namespace AB.Extensions
{
    /// <summary>
    /// Extensions for the <see cref="Int32"/> class.
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Returns the first ordinal digit for a given <see cref="Int32"/>.
        /// </summary>
        /// <param name="value"></param>
        public static int FirstDigit(this int value)
        {
            if (value == Int32.MinValue) return 2;
            int firstDigit = (int)(Math.Abs(value).ToString()[0]) - 48;
            return firstDigit;
        }

        // todo:
        // int64?
        // extra methods for return types for micro digits? (first digit is always 1-9
        // fix console extensions comments
        // datetime tests - crush some tests into [Theory]
    }
}
