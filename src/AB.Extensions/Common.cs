using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB.Extensions
{
    public static class Common
    {
        /// <summary>
        /// Enumeration that defines values representing valid ordering directions for a sequence.
        /// </summary>
        public enum OrderByDirection
        {
            /// <summary>
            /// Elements are ordered by increasing value
            /// </summary>
            Ascending = 0,
            /// <summary>
            /// Elements are ordered by decreasing value
            /// </summary>
            Descending = 1,
        }

        /// <summary>
        /// Enumeration that defines the four major cardinal directions.
        /// </summary>
        public enum CardinalDirection
        {
            Unspecified = 0,
            North = 1,
            South = 2,
            East = 3,
            West = 4
        }
    }
}
