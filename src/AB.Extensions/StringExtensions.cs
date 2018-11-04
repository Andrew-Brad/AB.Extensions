using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AB.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// The Regex used in <see cref="SplitQuotedCsv"/>.
        /// </summary>
        public static readonly Regex CsvSplitRegex = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);

        /// <summary>
        /// Will do a normal string split by <see cref="Common.StringConstants.Delimiters.CommaChar"/>, but will respect quoted 
        /// pieces of the strings and keep them together.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //Attribution: https://stackoverflow.com/questions/3776458/split-a-comma-separated-string-with-both-quoted-and-unquoted-strings
        public static IEnumerable<string> SplitQuotedCsv(this string input)
        {
            if (input == null) yield break;
            if (input.Length == 0) yield break;
            foreach (Match match in CsvSplitRegex.Matches(input))
            {
                yield return match.Value.TrimStart(Common.StringConstants.Delimiters.CommaChar);
            }
        }

        public static IEnumerable<string> SplitStringByLineBreaks(this string str, bool removeEmptyLines = false)
        {
            return str.Split(new[] { "\r\n", "\r", "\n" },
                removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static string RemoveLineBreaks(this string lines)
        {
            return lines.Replace("\r", "").Replace("\n", "");
        }

        public static string ReplaceLineBreaks(this string lines, string replacement)
        {
            return lines.Replace("\r\n", replacement)
                        .Replace("\r", replacement)
                        .Replace("\n", replacement);
        }

        /// <summary>
        /// A shorthand function for null checking, handy for some lambda operations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj == null;
        }

        public static bool IsNull<T>(this T? obj) where T : struct
        {
            return !obj.HasValue;
        }

                
        // Source: http://stackoverflow.com/questions/489258/linq-distinct-on-a-particular-property
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Reverse a string of characters, echoing null and empties.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToReverseString(this string input)
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
        /// <param name="input"></param>
        /// <param name="throwExceptionIfInvalid"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string input, bool throwExceptionIfInvalid = false)
        {
            Guid outGuid = Guid.Empty;
            if (Guid.TryParse(input,out outGuid) == true)
            {
                return outGuid;
            }
            else
            {
                if (throwExceptionIfInvalid) throw new FormatException("String is not formatted as a Guid.");
                else return Guid.Empty;
            }
        }

        public static T ToEnumTypeOf<T>(this string stringValue)
        {
            return (T)Enum.Parse(typeof(T), stringValue, true);
        }

        public static int CountOccurrencesOf(this string text, string occurrenceString)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            int count = 0;
            int i = 0;
            while ((i = text.IndexOf(occurrenceString, i)) != -1)
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
            return (fileSize + Common.StringConstants.Delimiters.Space + suffix[j]);
        }
    }
}
