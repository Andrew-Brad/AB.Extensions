using System.Text.RegularExpressions;

using static AB.Extensions.StringConstants;

namespace AB.Extensions;

/// <summary>
/// Extension methods and constants for working with <see cref="string"/> — line endings,
/// CSV splitting, null checks, and small conversions.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// The Regex used in <see cref="SplitQuotedCsv"/>.
    /// </summary>
    public static readonly Regex CsvSplitRegex = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);

    /// <summary>
    /// Will do a normal string split by <see cref="StringConstants.Delimiters.CommaChar"/>, but will respect quoted
    /// pieces of the strings and keep them together. Found @ https://stackoverflow.com/questions/3776458/split-a-comma-separated-string-with-both-quoted-and-unquoted-strings
    /// </summary>
    /// <param name="input">The (possibly null or empty) comma-separated string to split.</param>
    /// <returns>The split fields, with quoted segments preserved; empty when <paramref name="input"/> is null or empty.</returns>
    public static IEnumerable<string> SplitQuotedCsv(this string? input)
    {
        if (input == null) yield break;
        if (input.Length == 0) yield break;
        foreach (Match match in CsvSplitRegex.Matches(input))
        {
            yield return match.Value.TrimStart(StringConstants.Delimiters.CommaChar);
        }
    }

    /// <summary>
    /// Perform a string.Split() operation, respecting all line break (NewLine) representations across Mac, Windows, and Unix.
    /// </summary>
    /// <param name="str">The string to split.</param>
    /// <param name="removeEmptyLines">When true, empty entries produced by consecutive line breaks are removed.</param>
    /// <returns>The lines of <paramref name="str"/>, split on any OS line ending.</returns>
    public static string[] SplitStringByLineBreaks(this string str, bool removeEmptyLines = false)
    {
        return str.Split(AllOsLineEndings,
            removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    }

    /// <summary>
    /// Removes Mac (CR) and Unix (LF) line breaks from a string, leaving the remaining text concatenated.
    /// </summary>
    /// <param name="lines">The text to strip line breaks from.</param>
    /// <returns>The text with CR and LF characters removed.</returns>
    public static string RemoveLineBreaks(this string lines) => lines.Replace(MacLineEnding, string.Empty).Replace(UnixLineEnding, string.Empty);

    /// <summary>
    /// Replaces every Windows (CRLF), Mac (CR) and Unix (LF) line break in a string with the given replacement.
    /// </summary>
    /// <param name="lines">The text to operate on.</param>
    /// <param name="replacement">The string to substitute for each line break.</param>
    /// <returns>The text with all line endings replaced by <paramref name="replacement"/>.</returns>
    public static string ReplaceLineBreaks(this string lines, string replacement)
    {
        return lines.Replace(WindowsLineEnding, replacement)
                    .Replace(MacLineEnding, replacement)
                    .Replace(UnixLineEnding, replacement);
    }

    /// <summary>
    /// Reference-type null check, usable as a first-class predicate (e.g. a LINQ <c>Func&lt;T, bool&gt;</c>)
    /// where the <c>is null</c> operator can't be passed — and as a runtime check independent of the
    /// caller's nullable-reference-type context.
    /// </summary>
    /// <typeparam name="T">A reference type.</typeparam>
    /// <param name="obj">The object to test.</param>
    /// <returns>True if <paramref name="obj"/> is null.</returns>
    public static bool IsNull<T>(this T? obj) where T : class => obj is null;

    /// <summary>
    /// Nullable value-type null check, usable as the same first-class predicate. Orthogonal to nullable
    /// reference types — it tests <see cref="System.Nullable{T}.HasValue"/>.
    /// </summary>
    /// <typeparam name="T">A value type.</typeparam>
    /// <param name="obj">The nullable value to test.</param>
    /// <returns>True if <paramref name="obj"/> has no value.</returns>
    public static bool IsNull<T>(this T? obj) where T : struct => !obj.HasValue;


    /// <summary>
    /// Reverse a string of characters, echoing null and empties.
    /// </summary>
    /// <param name="input">The string to reverse.</param>
    /// <returns>The reversed string, or <paramref name="input"/> unchanged when it is null or empty.</returns>
    public static string? ToReverseString(this string? input)
    {
        if (input == null) return null;
        if (input.Length == 0) return input;
        char[] array = input.ToCharArray();
        Array.Reverse(array);
        return new string(array);
    }

    /// <summary>
    /// One liner for converting strings to Guids. Null safe.
    /// </summary>
    /// <param name="input">The candidate Guid string.</param>
    /// <param name="throwExceptionIfInvalid">When true, throws <see cref="FormatException"/> instead of returning <see cref="Guid.Empty"/> for invalid input.</param>
    /// <returns>The parsed <see cref="Guid"/>, or <see cref="Guid.Empty"/> when invalid and not throwing.</returns>
    public static Guid ToGuid(this string? input, bool throwExceptionIfInvalid = false)
    {
        Guid outGuid = Guid.Empty;
        if (Guid.TryParse(input, out outGuid) == true)
        {
            return outGuid;
        }
        else
        {
            if (throwExceptionIfInvalid) throw new FormatException("String is not formatted as a Guid.");
            else return Guid.Empty;
        }
    }

    /// <summary>
    /// Parses a string into the named member of an enum, ignoring case.
    /// </summary>
    /// <typeparam name="T">The enum type to parse into.</typeparam>
    /// <param name="stringValue">The case-insensitive enum member name.</param>
    /// <returns>The matching enum value.</returns>
    public static T ToEnumTypeOf<T>(this string stringValue) => (T)Enum.Parse(typeof(T), stringValue, true);

    /// <summary>
    /// Counts the non-overlapping occurrences of a substring within a string.
    /// </summary>
    /// <param name="text">The text to search; null or empty yields zero.</param>
    /// <param name="occurrenceString">The substring to count.</param>
    /// <returns>The number of times <paramref name="occurrenceString"/> appears in <paramref name="text"/>.</returns>
    public static int CountOccurrencesOf(this string? text, string occurrenceString)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        int count = 0;
        int i = 0;
        while ((i = text!.IndexOf(occurrenceString, i)) != -1)
        {
            i += occurrenceString.Length;
            count++;
        }
        return count;
    }

    private const string bytesString = "bytes";
    private const string kilobytesString = "KB";
    private const string megabytesString = "MB";
    private const string gigabytesString = "GB";
    private static readonly string[] suffix = { bytesString, kilobytesString, megabytesString, gigabytesString };

    /// <summary>
    /// Given a number of bytes, return a string that has a nicer display value for the total in terms of kilobytes, megabytes, and gigabytes.
    /// Assumes the IEC (non- SI) form where 1024 bytes = 1 MB.  Negative storage values don't make sense.
    /// </summary>
    /// <param name="fileSize"></param>
    /// <returns>Text like '16 GB', or '128 MB'.</returns>
    // Attribution: https://stackoverflow.com/questions/128618/file-size-format-provider
    public static string FileSizeString(this ulong fileSize)
    {
        long j = 0;

        while (fileSize > 1024 && j < 4)
        {
            fileSize = fileSize / 1024;
            j++;
        }
        return (fileSize + StringConstants.Delimiters.Space + suffix[j]);
    }
}
