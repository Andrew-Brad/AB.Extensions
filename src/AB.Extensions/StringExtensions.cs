using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AB.Extensions
{
    public static class Extensions
    {
        public static IEnumerable<string> SplitQuotedCSV(this string input)  //credit: http://stackoverflow.com/questions/3776458/split-a-comma-separated-string-with-both-quoted-and-unquoted-strings
        {
            Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);

            foreach (Match match in csvSplit.Matches(input))
            {
                //Interesting explanation of yield return here: https://www.youtube.com/watch?v=4fju3xcm21M
                //I think it may actually be more performant than copying an array back and forth or using LINQ to return
                yield return match.Value.TrimStart(',');
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
        public static bool IsNull<T>(this T obj) where T : class
        {
            return obj == null;
        }
        public static bool IsNull<T>(this T? obj) where T : struct
        {
            return !obj.HasValue;
        }

        #region Randoms

        //grabbed from http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
        public static class ThreadSafeRandom
        {
            [ThreadStatic]
            private static Random Local;

            public static Random ThisThreadsRandom
            {
                get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
            }
        }


        public static void NaiveNonRandomShuffle<T>(this IList<T> list)
        {
            list.OrderBy(a => Guid.NewGuid());
        }               

        #endregion Randoms
                
        //Source: http://stackoverflow.com/questions/489258/linq-distinct-on-a-particular-property
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
        public static string ToReverseString(this string text)
        {
            if (text == null) return null;
            if (text == string.Empty) return string.Empty;
            char[] array = text.ToCharArray();
            Array.Reverse(array);
            return new String(array);
        }

        /// <summary>
        /// One liner for converting strings to Guids.
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
    }
}
