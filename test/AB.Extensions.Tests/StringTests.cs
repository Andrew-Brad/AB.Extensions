using Xunit;

// Namespace aligned with every other test file (was the odd-one-out "ABExtensions.Tests").
// The extension methods live in the parent AB.Extensions namespace, visible here without a using;
// ImplicitUsings covers System, System.Collections.Generic, System.IO and System.Linq.
namespace AB.Extensions.Tests;

public class StringTests
{
    // --- ToReverseString: null and empty echo through unchanged ---

    [Theory]
    [InlineData("hi!", "!ih")]
    [InlineData("weRdNab", "baNdRew")]
    [InlineData("", "")]      // empty echoes
    [InlineData(null, null)]  // null echoes
    public void ToReverseString_ReversesCharacters(string? input, string? expected) =>
        Assert.Equal(expected, input.ToReverseString());

    // --- ToGuid: valid input parses regardless of the throw flag ---

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ToGuid_ValidString_Parses(bool throwOnInvalid)
    {
        const string input = "d2650790-bc80-41a9-ae63-dd55e2240296";
        Assert.Equal(Guid.Parse(input), input.ToGuid(throwOnInvalid));
    }

    // Invalid input is forgiving by default — Guid.Empty rather than an exception.
    [Theory]
    [InlineData("~~~~~d2sdd650790-bc80-41a9-ae63-dd55e2240296")]
    [InlineData("")]
    [InlineData(null)]
    public void ToGuid_InvalidString_ReturnsEmpty_ByDefault(string? input) =>
        Assert.Equal(Guid.Empty, input.ToGuid());

    // ...unless the caller opts into throwing.
    [Fact]
    public void ToGuid_InvalidString_Throws_WhenRequested() =>
        Assert.Throws<FormatException>(() => "not-a-guid".ToGuid(throwExceptionIfInvalid: true));

    // --- ToEnumTypeOf: case-insensitive name parse ---

    [Theory]
    [InlineData("Ascending", OrderByDirection.Ascending)]
    [InlineData("descending", OrderByDirection.Descending)] // parse is case-insensitive
    public void ToEnumTypeOf_ParsesMemberName(string input, OrderByDirection expected) =>
        Assert.Equal(expected, input.ToEnumTypeOf<OrderByDirection>());

    [Fact]
    public void ToEnumTypeOf_UnknownName_Throws() =>
        Assert.Throws<ArgumentException>(() => "lol".ToEnumTypeOf<OrderByDirection>());

    // --- CountOccurrencesOf: non-overlapping count, null/empty-safe ---

    [Theory]
    [InlineData("lol", "l", 2)]
    [InlineData("banana", "ana", 1)]  // non-overlapping: the second "ana" overlaps the first, so it isn't counted
    [InlineData("aaaa", "aa", 2)]     // non-overlapping run
    [InlineData("abc", "z", 0)]       // needle absent
    [InlineData("", "l", 0)]          // empty text
    [InlineData("abc", "", 0)]        // empty needle can't match (and must not spin forever)
    [InlineData(null, "l", 0)]        // null text
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
    [InlineData(1023, "1023 bytes")]        // just under the first boundary
    [InlineData(1024, "1 KB")]              // exact boundary rolls up to the next unit
    [InlineData(1048576, "1 MB")]           // 1024^2: exact MB boundary
    [InlineData(1073741824, "1 GB")]        // 1024^3: exact GB boundary
    [InlineData(5497558138880, "5120 GB")]  // 5 TB: caps at GB instead of indexing past `suffix`
    public void FileSizeString_FormatsWithIecUnits(long byteCount, string expected) =>
        Assert.Equal(expected, ((ulong)byteCount).FileSizeString());

    // --- SplitQuotedCsv: splits on commas but keeps quoted segments intact ---
    // Assert the actual fields, not just the count, so distinct inputs can't pass interchangeably.

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

    // --- SplitStringByLineBreaks: on-disk fixtures pinned byte-for-byte in .gitattributes ---
    // (eol=lf / eol=crlf / -text) so each fixture keeps the ending it claims across the git
    // round-trip on every platform — unlike the old `* text=auto`, which normalized them all to
    // LF and let the "Windows" case silently pass while actually testing Unix endings.
    [Theory]
    [InlineData("windows_line_ending.txt", "\r\n")]
    [InlineData("unix_line_ending.txt", "\n")]
    [InlineData("mac_line_ending.txt", "\r")]
    public void SplitStringByLineBreaks_FixturesHoldTheirBytesAndSplitIntoThreeLines(string fileName, string lineEnding)
    {
        string content = File.ReadAllText(fileName);

        // The fixture genuinely holds the ending it claims — not a normalized substitute.
        Assert.Contains(lineEnding, content);
        if (lineEnding == StringConstants.UnixLineEnding) Assert.DoesNotContain("\r", content);
        if (lineEnding == StringConstants.MacLineEnding) Assert.DoesNotContain("\n", content);

        // Content, not just count, so the three cases can't pass interchangeably.
        Assert.Equal(new[] { "line1", "line2", "line3" }, content.SplitStringByLineBreaks());
    }

    // In-memory split across all three OS endings, built from the constants themselves.
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

    // --- RemoveLineBreaks / ReplaceLineBreaks: strip or substitute all three endings ---
    // CRLF is the interesting case: stripping \r and \n individually already collapses it,
    // so the explicit CRLF pass is for symmetry between the two methods.
    [Theory]
    [InlineData("line1\r\nline2\r\nline3", "line1line2line3")]
    [InlineData("line1\rline2\rline3", "line1line2line3")]
    [InlineData("line1\nline2\nline3", "line1line2line3")]
    [InlineData("no breaks here", "no breaks here")]
    [InlineData("", "")]
    public void RemoveLineBreaks_StripsAllEndings(string input, string expected) =>
        Assert.Equal(expected, input.RemoveLineBreaks());

    [Theory]
    [InlineData("line1\r\nline2", "line1 line2")]
    [InlineData("line1\rline2", "line1 line2")]
    [InlineData("line1\nline2", "line1 line2")]
    [InlineData("no breaks here", "no breaks here")]
    public void ReplaceLineBreaks_SubstitutesAllEndings(string input, string expected) =>
        Assert.Equal(expected, input.ReplaceLineBreaks(" "));
}
