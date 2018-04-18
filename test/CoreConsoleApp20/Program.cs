using System;
using System.Threading.Tasks;
using static AB.Extensions.ConsoleExtensions;

namespace CoreConsoleApp20
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLineWithColor("Printing in yellow.", ConsoleColor.Yellow);
            WriteLineWithColor("Printing in green.", ConsoleColor.Green);

            WriteLineWithColor("Cursor Left: " + Console.CursorLeft + ".  Cursor top:" + Console.CursorTop, ConsoleColor.White);
            WriteToBottomLineWithColor("Setting time...", ConsoleColor.Red);
            WriteToBottomLineWithColor("Corner case..." + new string('s',900000), ConsoleColor.Red);
            ClearConsoleLine(1);

            //Infinite task to print time at the bottom:
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(2000);
                    WriteToBottomLineWithColor(DateTime.Now.ToLongTimeString(), ConsoleColor.DarkRed);
                }
            });
            Console.ReadKey();
        }

    }
}
