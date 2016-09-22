using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AB.Extensions
{
    public class ConsoleExtensions
    {
        public static void WriteLineWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
