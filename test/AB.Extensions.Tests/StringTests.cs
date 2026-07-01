using Xunit;

// Extension methods live in the parent AB.Extensions namespace; ImplicitUsings covers the System usings.
namespace AB.Extensions.Tests;

public class StringTests
{
    // --- ToReverseString ---

    [Theory]
    [InlineData("hi!", "!ih")]
    [InlineData("weRdNab", "baNdRew")]
    [InlineData("", "")]      // empty echoes
    [InlineData(null, null)]  // null echoes
    public void ToReverseString_ReversesCharacters(string? input, string? expected) =>
        Assert.Equal(expected, input.ToReverseString());

    // --- ToGuid ---

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToGuid_ValidString_Parses(bool throwOnInvalid)
    {
        const string input = "d2650790-bc80-41a9-ae63-dd55e2240296";
        Assert.Equal(Guid.Parse(input), input.ToGuid(throwOnInvalid));
    }

    // Forgiving by default: Guid.Empty rather than throwing.
    [Theory]
    [InlineData("~~~~~d2sdd650790-bc80-41a9-ae63-dd55e2240296")]
    [InlineData("")]
    [InlineData(null)]
    public void ToGuid_InvalidString_ReturnsEmpty_ByDefault(string? input) =>
        Assert.Equal(Guid.Empty, input.ToGuid());

    [Fact]
    public void ToGuid_InvalidString_Throws_WhenRequested() =>
        Assert.Throws<FormatException>(() => "not-a-guid".ToGuid(throwExceptionIfInvalid: true));

    // --- ToEnumTypeOf ---

    [Theory]
    [InlineData("Ascending", OrderByDirection.Ascending)]
    [InlineData("descending", OrderByDirection.Descending)] // case-insensitive
    public void ToEnumTypeOf_ParsesMemberName(string input, OrderByDirection expected) =>
        Assert.Equal(expected, input.ToEnumTypeOf<OrderByDirection>());

    [Fact]
    public void ToEnumTypeOf_UnknownName_Throws() =>
        Assert.Throws<ArgumentException>(() => "lol".ToEnumTypeOf<OrderByDirection>());

    // --- CountOccurrencesOf ---

    [Theory]
    [InlineData("lol", "l", 2)]
    [InlineData("banana", "ana", 1)]  // non-overlapping
    [InlineData("aaaa", "aa", 2)]
    [InlineData("abc", "z", 0)]
    [InlineData("", "l", 0)]
    [InlineData("abc", "", 0)]        // empty needle: 0, and must not spin forever
    [InlineData(null, "l", 0)]
    public void CountOccurrencesOf_CountsNonOverlapping(string? text, string needle, int expected) =>
        Assert.Equal(expected, text.CountOccurrencesOf(needle));

    // --- FileSizeString: IEC (1024) units, capping at GB ---
    // https://catalystsecure.com/blog/2011/05/how-many-bytes-in-a-gigabyte-my-answer-might-surprise-you/
    [Theory]
    [InlineData(128, "128 bytes")]
    [InlineData(17179869184, "16 GB")]
    [InlineData(4194304, "4 MB")]
    [InlineData(46080, "45 KB")]
    [InlineData(0, "0 bytes")]
    [InlineData(1023, "1023 bytes")]        // just under the boundary
    [InlineData(1024, "1 KB")]              // exact boundary rolls up
    [InlineData(1048576, "1 MB")]
    [InlineData(1073741824, "1 GB")]
    [InlineData(5497558138880, "5120 GB")]  // 5 TB caps at GB
    public void FileSizeString_FormatsWithIecUnits(long byteCount, string expected) =>
        Assert.Equal(expected, ((ulong)byteCount).FileSizeString());

    // --- SplitQuotedCsv: assert fields, not just count ---

    [Fact]
    public void SplitQuotedCsv_KeepsQuotedCommasTogether() =>
        Assert.Equal(
            new[] { "111", "222", "\"33,44,55\"", "666", "\"77,88\"", "\"99\"" },
            @"111,222,""33,44,55"",666,""77,88"",""99""".SplitQuotedCsv());

    [Fact]
    public void SplitQuotedCsv_PlainCsv_SplitsEveryField() =>
        Assert.Equal(new[] { "1", "2", "3", "4", "5" }, "1,2,3,4,5".SplitQuotedCsv());

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void SplitQuotedCsv_NullOrEmpty_YieldsNothing(string? input) =>
        Assert.Empty(input.SplitQuotedCsv());

    // --- SplitStringByLineBreaks ---
    // Fixtures are byte-pinned in .gitattributes (eol=lf/crlf/-text) so each keeps its ending across the git round-trip.
    [Theory]
    [InlineData("windows_line_ending.txt", "\r\n")]
    [InlineData("unix_line_ending.txt", "\n")]
    [InlineData("mac_line_ending.txt", "\r")]
    public void SplitStringByLineBreaks_FixturesHoldTheirBytesAndSplitIntoThreeLines(string fileName, string lineEnding)
    {
        string content = File.ReadAllText(fileName);

        // Confirm the fixture really holds the ending it claims.
        Assert.Contains(lineEnding, content);
        if (lineEnding == StringConstants.UnixLineEnding) Assert.DoesNotContain("\r", content);
        if (lineEnding == StringConstants.MacLineEnding) Assert.DoesNotContain("\n", content);

        Assert.Equal(new[] { "line1", "line2", "line3" }, content.SplitStringByLineBreaks());
    }

    [Theory]
    [InlineData(StringConstants.WindowsLineEnding)]
    [InlineData(StringConstants.UnixLineEnding)]
    [InlineData(StringConstants.MacLineEnding)]
    public void SplitStringByLineBreaks_SplitsEveryOsEnding(string ending) =>
        Assert.Equal(new[] { "line1", "line2", "line3" }, ("line1" + ending + "line2" + ending + "line3").SplitStringByLineBreaks());

    [Fact]
    public void SplitStringByLineBreaks_RemoveEmptyEntries_DropsBlankLines()
    {
        string content = "line1" + StringConstants.WindowsLineEnding
                       + StringConstants.WindowsLineEnding
                       + StringConstants.WindowsLineEnding + "line3";

        Assert.Equal(new[] { "line1", "line3" }, content.SplitStringByLineBreaks(removeEmptyLines: true));
    }

    // --- RemoveLineBreaks / ReplaceLineBreaks ---

    [Theory]
    [InlineData("line1\r\nline2\r\nline3", "line1line2line3")]
    [InlineData("line1\rline2\rline3", "line1line2line3")]
    [InlineData("line1\nline2\nline3", "line1line2line3")]
    [InlineData("no breaks here", "no breaks here")]
    [InlineData("", "")]
    public void RemoveLineBreaks_StripsAllEndings(string input, string expected) =>
        Assert.Equal(expected, input.RemoveLineBreaks());

    // CRLF replaced as one unit (one space), not \r + \n separately (two).
    [Theory]
    [InlineData("line1\r\nline2", "line1 line2")]
    [InlineData("line1\rline2", "line1 line2")]
    [InlineData("line1\nline2", "line1 line2")]
    [InlineData("no breaks here", "no breaks here")]
    public void ReplaceLineBreaks_SubstitutesAllEndings(string input, string expected) =>
        Assert.Equal(expected, input.ReplaceLineBreaks(" "));
}
