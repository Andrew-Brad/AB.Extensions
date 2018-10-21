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
        public static int LeadingDigit(this int value)
        {
            if (value < 0 && (value = -value) < 0) return 2;
            return (value < 100) ? (value < 1) ? 0 : (value < 10)
                    ? value : value / 10 : (value < 1000000) ? (value < 10000)
                    ? (value < 1000) ? value / 100 : value / 1000 : (value < 100000)
                    ? value / 10000 : value / 100000 : (value < 100000000)
                    ? (value < 10000000) ? value / 1000000 : value / 10000000
                    : (value < 1000000000) ? value / 100000000 : value / 1000000000;
        }

        // todo:
        // int64?
        // extra methods for return types for micro digits? (first digit is always 1-9
        // fix console extensions comments
        // datetime tests - crush some tests into [Theory]
    }
}
