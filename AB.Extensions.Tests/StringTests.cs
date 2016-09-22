using Xunit;
using AB.Extensions;

namespace ABExtensions.Tests
{
    public class StringTests
    {
        [Fact]
        public void ReverseString_1()
        {
            string input = "hi!";
            string expected = "!ih";
            string operation = input.ToReverseString();
            Assert.Equal(expected, operation);
        }

        [Fact]
        public void ReverseString_2()
        {
            string input = "weRdNab";
            string expected = "baNdRew";
            string operation = input.ToReverseString();
            Assert.Equal(expected, operation);
        }

        [Fact]
        public void ReverseString_3()
        {
            string input = "";
            string expected = string.Empty;
            string operation = input.ToReverseString();
            Assert.Equal(expected, operation);
        }

        [Fact]
        public void ReverseString_4()
        {
            string input = null;
            string expected = null;
            string operation = input.ToReverseString();
            Assert.Equal(expected, operation);
        }
    }
}
