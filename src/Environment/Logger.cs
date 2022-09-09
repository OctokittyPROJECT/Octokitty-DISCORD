using System;
using System.Text;

namespace Octokitty.Environment
{
    internal class Logger
    {
        public static void Info(string message)
        {
            string log_entry = "[INFO/ " + message;

            Console.ForegroundColor 
                = ConsoleColor.Gray;

            Console.WriteLine(log_entry);

            Console.ResetColor();
        }

        public static void Warn(string message)
        {
            string log_entry = "[WARN/ " + message;

            Console.ForegroundColor
                = ConsoleColor.Yellow;

            Console.WriteLine(log_entry);

            Console.ResetColor();
        }

        public static void Error(string message)
        {
            string log_entry = "[ERROR/ " + message;

            Console.ForegroundColor
                = ConsoleColor.Red;

            Console.WriteLine(log_entry);

            Console.ResetColor();
        }
    }
}