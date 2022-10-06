using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctokittyBOT
{
    public static class Logger
    {
        /// <summary>
        /// Writes in console via green-colored text and can write it in logs session file with time formattings intended.
        /// </summary>
        /// <param name="message">
        /// A string respresenting console and logs output
        /// </param>
        /// <param name="type">
        /// A string representing custom tag of category of console message
        /// </param>
        /// <param name="logInFile">
        /// A boolean value which defines, will function append logs message in audit file
        /// </param>
        public static void Success(string message, string type, bool logInFile = true)
        {
            type = type.ToUpper();

            string logsFile = $"logs-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

            string logsMessage = $"[{type}/SUCCESS] {DateTime.Now.ToString("HH/mm/ss")} {message}";

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(logsMessage);

            Console.ResetColor();

            if(logInFile)
            {
                if (!File.Exists(Path.Combine("logs/", logsFile)))
                    PrepareLogsFile();

                File.AppendAllText(Path.Combine("logs/", logsFile), logsMessage);
            }
        }

        /// <summary>
        /// Writes in console via yellow-colored text and can write it in logs session file with time formattings intended.
        /// </summary>
        /// <param name="message">
        /// A string respresenting console and logs output
        /// </param>
        /// <param name="type">
        /// A string representing custom tag of category of console message
        /// </param>
        /// <param name="logInFile">
        /// A boolean value which defines, will function append logs message in audit file
        /// </param>
        public static void Warn(string message, string type, bool logInFile = true)
        {
            type = type.ToUpper();

            string logsFile = $"logs-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

            string logsMessage = $"[{type}/WARN] {DateTime.Now.ToString("HH/mm/ss")} {message}";

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(logsMessage);

            Console.ResetColor();

            if (logInFile)
            {
                if (!File.Exists(Path.Combine("logs/", logsFile)))
                    PrepareLogsFile();

                File.AppendAllText(Path.Combine("logs/", logsFile), logsMessage);
            }
        }

        /// <summary>
        /// Writes in console via red-colored text and can write it in logs session file with time formattings intended.
        /// </summary>
        /// <param name="message">
        /// A string respresenting console and logs output
        /// </param>
        /// <param name="type">
        /// A string representing custom tag of category of console message
        /// </param>
        /// <param name="logInFile">
        /// A boolean value which defines, will function append logs message in audit file
        /// </param>
        public static void Error(string message, string type, bool logInFile = true)
        {
            type = type.ToUpper();

            string logsFile = $"logs-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

            string logsMessage = $"[{type}/ERROR] {DateTime.Now.ToString("HH/mm/ss")} {message}";

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(logsMessage);

            Console.ResetColor();

            if (logInFile)
            {
                if (!File.Exists(Path.Combine("logs/", logsFile)))
                    PrepareLogsFile();

                File.AppendAllText(Path.Combine("logs/", logsFile), logsMessage);
            }
        }

        /// <summary>
        /// Writes in console via white-colored text and can write it in logs session file with time formattings intended.
        /// </summary>
        /// <param name="message">
        /// A string respresenting console and logs output
        /// </param>
        /// <param name="type">
        /// A string representing custom tag of category of console message
        /// </param>
        /// <param name="logInFile">
        /// A boolean value which defines, will function append logs message in audit file
        /// </param>
        public static void Info(string message, string type, bool logInFile = true)
        {
            type = type.ToUpper();

            string logsFile = $"logs-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

            string logsMessage = $"[{type}/INFO] {DateTime.Now.ToString("HH/mm/ss")} {message}";

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(logsMessage);

            Console.ResetColor();

            if (logInFile)
            {
                if (!File.Exists(Path.Combine("logs/", logsFile)))
                    PrepareLogsFile();

                File.AppendAllText(Path.Combine("logs/", logsFile), logsMessage);
            }
        }

        /// <summary>
        /// Prepares logs file for its future type-sessions
        /// </summary>
        private static void PrepareLogsFile()
        {
            if (!Directory.Exists("logs"))
                Directory.CreateDirectory("logs");

            string logsFile = $"logs-{DateTime.Now.ToString("dd/MM/yyyy")}.logs";

            if(!File.Exists(Path.Combine("logs/", logsFile)))
            {
                using (FileStream stream = File.Create(Path.Combine("logs/", logsFile)))
                {
                    string firstLogs = $"[INIT/INFO] {DateTime.Now.ToString("HH/mm/ss")} Created logs session successfully!";

                    byte[] logsBytes = Encoding.UTF8.GetBytes(firstLogs);

                    stream.Write(logsBytes, 0, logsBytes.Length);
                    stream.Close();
                }
            }
        }
    }
}
