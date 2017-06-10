using System;
using AB.Extensions;
using Xunit;

namespace ABExtensions.Tests
{
    public class DateTimeExtensionsTests
    {
        [Fact]
        public void IsBetween_1()
        {
            Assert.True(DateTime.Now.IsBetween(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1)));
        }

        [Fact]
        public void IsBetween_2()
        {
            bool isBet = DateTime.Now.IsBetween(DateTime.Now.AddSeconds(1), DateTime.Now.AddSeconds(2), true);
            Assert.False(isBet);
        }

        [Fact]
        public void IsBetween_3()
        {
            Assert.False(DateTime.Now.IsBetween(DateTime.Now.AddSeconds(1), DateTime.Now.AddSeconds(1), true));
        }

        [Fact]
        public void IsBetween_4()//identical dates are considered between each other
        {
            Assert.True(DateTime.MinValue.IsBetween(DateTime.MinValue, DateTime.MinValue, true));
        }

        [Fact]
        public void LeapYear_1()//identical dates are considered between each other
        {
            Assert.True(DateTime.IsLeapYear(2000));
        }

        [Fact]
        public void LeapYear_2()
        {
            Assert.False(DateTime.IsLeapYear(1921));
        }

        [Fact]
        public void LeapYear_3()
        {
            Assert.False(DateTime.IsLeapYear(1900));
        }


    }
}
