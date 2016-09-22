using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB.Extensions
{
    public static class NumericalExtensions
    {
        private const string bytesString = "bytes";
        private const string kilobytesString = "KB";
        private const string megabytesString = "MB";
        private const string gigabytesString = "GB";

        public static string FormatFileSize(this long fileSize)
        {
            string[] suffix = { bytesString, kilobytesString, megabytesString, gigabytesString };
            long j = 0;

            while (fileSize > 1024 && j < 4)
            {
                fileSize = fileSize / 1024;
                j++;
            }
            return (fileSize + " " + suffix[j]);
        }
    }
}
