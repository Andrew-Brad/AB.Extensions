using System;
using static AB.Extensions.Common.StringConstants;

namespace AB.Extensions
{
    public class ConsoleExtensions
    {
        /// <summary>
        /// A simple <see cref="Console.WriteLine"/> but with a foreground color toggle to keep your code clean.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void WriteLineWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// A simple <see cref="Console.Write"/> but with a foreground color toggle to keep your code clean.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void WriteWithColor(string message, ConsoleColor color)
        {
            //Capture existing colors:
            var foreColor = Console.ForegroundColor;
            var backColor = Console.BackgroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = foreColor;
            Console.BackgroundColor = backColor;
        }
        
        /// <summary>
        /// Clears the current line at the cursor.
        /// </summary>
        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(Delimiters.Space.PadRight(Console.BufferWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        /// <summary>
        /// Clears the line at the given index as determined by <see cref="Console.CursorTop"/>.
        /// </summary>
        /// <param name="lineNumber">The line number from the top of the console.</param>
        public static void ClearConsoleLine(int lineNumber)
        {
            //Capture existing cursor position:
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            Console.SetCursorPosition(0, lineNumber);
            Console.Write(Delimiters.Space.PadRight(Console.BufferWidth - 1));
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        /// <summary>
        /// A wrapper function that writes to the bottom line of your console, resetting the cursor to its original position.
        /// If your text is wider than the console window, it will be truncated for presentation on screen.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public static void WriteToBottomLineWithColor(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            //Capture existing cursor position:
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;
            //Set cursor to bottom line:
            Console.CursorLeft = 0;
            Console.CursorTop = Console.WindowHeight - 1;
            //Clear line for corner cases:
            ClearCurrentConsoleLine();
            //Print Line:
            if (message.Length > Console.WindowWidth) WriteWithColor(message.Substring(0, Console.WindowWidth - 1), color);
            else WriteWithColor(message, color);
            //Reset to original position:
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }
}
