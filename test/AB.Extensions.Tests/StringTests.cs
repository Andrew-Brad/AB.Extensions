using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using AB.Extensions;
using AB.Extensions.Enums;

using Xunit;

namespace ABExtensions.Tests
{
    public class StringTests
    {
        [Theory]
        [InlineData("hi!", "!ih")]
        [InlineData("weRdNab", "baNdRew")]
        [InlineData("", "")]
        [InlineData(null, null)]

        public void ReverseString_1(string? input, string? expected)
        {
            // Act
            string? reversed = input.ToReverseString();

            // Assert
            Assert.Equal(expected, reversed);
        }

        [Fact]
        public void ToGuid_Valid_Throw_Bool()
        {
            //Arrange
            string input = "d2650790-bc80-41a9-ae63-dd55e2240296";
            Guid desiredOutput = Guid.Parse("d2650790-bc80-41a9-ae63-dd55e2240296");

            //Act
            var result = input.ToGuid(true);

            //Assert
            Assert.Equal(desiredOutput, result);
        }

        [Fact]
        public void ToGuid_Valid()
        {
            //Arrange
            string input = "d2650790-bc80-41a9-ae63-dd55e2240296";
            Guid desiredOutput = Guid.Parse("d2650790-bc80-41a9-ae63-dd55e2240296");

            //Act
            var result = input.ToGuid(false);

            //Assert
            Assert.Equal(desiredOutput, result);
        }

        [Theory]
        [InlineData("~~~~~d2sdd650790-bc80-41a9-ae63-dd55e2240296")]
        [InlineData("")]
        [InlineData(null)]
        public void ToGuid_Invalid_Cases(string? input)
        {
            //Arrange
            Guid desiredOutput = Guid.Empty;

            //Act
            var result = input.ToGuid();

            //Assert
            Assert.Equal(Guid.Empty, result);
        }

        [Fact]
        public void String_To_Enum_Type_Of()
        {
            //Arrange
            string input = "Ascending";
            OrderByDirection desiredOutput = OrderByDirection.Ascending;
            //Act
            var result = input.ToEnumTypeOf<OrderByDirection>();
            //Assert
            Assert.Equal(desiredOutput, result);
        }

        [Fact]
        public void String_To_Enum_Type_Of_Invalid()
        {
            //Arrange
            string input = "lol";
            //Act

            //Assert
            Assert.Throws<ArgumentException>(() => input.ToEnumTypeOf<OrderByDirection>());
        }

        [Fact]
        public void CountOccurrences()
        {
            //Arrange
            string input = "lol";
            int expectedCount = 2;
            //Act
            int actualCount = input.CountOccurrencesOf("l");
            //Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void CountOccurrences_empty_string_should_return_0()
        {
            //Arrange
            string input = "";
            int expectedCount = 0;
            //Act
            int actualCount = input.CountOccurrencesOf("l");
            //Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void CountOccurrences_empty_string_should_return_0_input_empty()
        {
            //Arrange
            string input = "";
            int expectedCount = 0;
            //Act
            int actualCount = input.CountOccurrencesOf("");
            //Assert
            Assert.Equal(expectedCount, actualCount);
        }

        //https://catalystsecure.com/blog/2011/05/how-many-bytes-in-a-gigabyte-my-answer-might-surprise-you/
        [Theory]
        [InlineData(128, "128 bytes")]
        [InlineData(17179869184, "16 GB")]
        [InlineData(4194304, "4 MB")]
        [InlineData(46080, "45 KB")]
        [InlineData(0, "0 bytes")]
        public void FormatFileSize(long byteCount, string expectedDisplayText)
        {
            // Arrange
            ulong input = (ulong)byteCount;

            // Act
            string text = input.FileSizeString();

            // Assert
            Assert.Equal(expectedDisplayText, text);
        }

        [Theory]
        [InlineData(@"1,2,3,4,5,6,7,8,9,10", 10)]
        [InlineData(@"111,222,""33,44,55"",666,""77,88"",""99""", 6)]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        public void SplitQuotedCsvString(string? input, int expectedCount)
        {
            // Act
            IEnumerable<string> split = input.SplitQuotedCsv();

            // Assert
            Assert.Equal(expectedCount, split.Count());
        }

        // These on-disk fixtures are byte-pinned in .gitattributes (eol=lf / eol=crlf / -text),
        // so the line ending each one claims survives the git round-trip on every platform —
        // unlike the old `* text=auto` behavior, which normalized them all to LF and let the
        // "Windows" case silently pass while testing Unix endings.
        [Theory]
        [InlineData("windows_line_ending.txt", "\r\n")]
        [InlineData("unix_line_ending.txt", "\n")]
        [InlineData("mac_line_ending.txt", "\r")]
        public void Cross_Platform_Line_Ending_Fixtures_Hold_Their_Bytes_And_Split_Into_Three_Lines(string fileName, string lineEnding)
        {
            // Arrange
            string content = File.ReadAllText(fileName);

            // Assert the fixture genuinely holds the ending it claims — not a normalized substitute.
            Assert.Contains(lineEnding, content);
            if (lineEnding == StringConstants.UnixLineEnding) Assert.DoesNotContain("\r", content);
            if (lineEnding == StringConstants.MacLineEnding) Assert.DoesNotContain("\n", content);

            // Act
            string[] split = content.SplitStringByLineBreaks();

            // Assert — content, not just count, so the three cases can't pass interchangeably.
            Assert.Equal(new[] { "line1", "line2", "line3" }, split);
        }

        [Fact]
        public void Windows_Line_Endings_Produce_Correct_Line_Count()
        {
            // Arrange
            string content = "line1" + StringConstants.WindowsLineEnding + "line2" + StringConstants.WindowsLineEnding + "line3";
            int expectedCount = 3;

            // Act
            string[] split = content.SplitStringByLineBreaks();
            int actualCount = split.Length;

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void Windows_Line_Endings_Produce_Correct_Line_Count_Removing_Empty()
        {
            // Arrange
            string content = "line1" + StringConstants.WindowsLineEnding + StringConstants.WindowsLineEnding + StringConstants.WindowsLineEnding + "line3";
            int expectedCount = 2;

            // Act
            string[] split = content.SplitStringByLineBreaks(true);
            int actualCount = split.Length;

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void Unix_Line_Endings_Produce_Correct_Line_Count()
        {
            // Arrange
            string content = "line1" + StringConstants.UnixLineEnding + "line2" + StringConstants.UnixLineEnding + "line3";
            int expectedCount = 3;

            // Act
            string[] split = content.SplitStringByLineBreaks();
            int actualCount = split.Length;

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public void Mac_Line_Endings_Produce_Correct_Line_Count()
        {
            // Arrange
            string content = "line1" + StringConstants.MacLineEnding + "line2" + StringConstants.MacLineEnding + "line3";
            int expectedCount = 3;

            // Act
            string[] split = content.SplitStringByLineBreaks();
            int actualCount = split.Length;

            // Assert
            Assert.Equal(expectedCount, actualCount);
        }
    }
}
