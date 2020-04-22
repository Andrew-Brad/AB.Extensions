using System;
using System.Threading.Tasks;
using static AB.Extensions.ConsoleExtensions;

namespace CoreConsoleApp20
{
    public class Program
    {
        static void Main(string[] args)
        {
            ClearCurrentConsoleLine(); // corner case

            foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor)))
            {
                WriteLineWithColor($"Printing in {color.ToString()}.", color);
            }

            ClearConsoleLine(0); //black

            WriteLineWithColor($"Cursor position: [{Console.CursorLeft},{Console.CursorTop}]", ConsoleColor.Green);
            WriteLineWithColor("                                                         ", ConsoleColor.Black, ConsoleColor.DarkYellow);
            WriteToBottomLineWithColor("Setting time...", ConsoleColor.Red);
            WriteToBottomLineWithColor("Corner case..." + new string('s', 900000), ConsoleColor.Red);
            ClearCurrentConsoleLine();

            //Infinite task to print time at the bottom:
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(1000);
                    WriteToBottomLineWithColor(DateTime.Now.ToLongTimeString(), ConsoleColor.DarkRed);
                }
            });
            Console.ReadKey();
        }
    }
}
