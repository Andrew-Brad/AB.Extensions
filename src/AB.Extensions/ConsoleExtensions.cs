using System;

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
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
        }

        /// <summary>
        /// A wrapper function that writes to the bottom line of your console, resetting the cursor to its original position.
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
            //Print Line:
            WriteWithColor(message, color);
            //Reset to original position:
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }
}
