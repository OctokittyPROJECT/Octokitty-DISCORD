using System;
using System.Text;

using Octokitty.Coroutines;

namespace Octokitty.Environments
{
    internal static class Logger
    {
        /*
         * Introduction to the custom logging service:
         * 
         * APIs with which this bot works in some cases respond with big either unsupported for human eye formatting,
         * thats why this service exists: provide direct associations for user using it with globally accepted colors
         * alike green or red.
         * 
         * Also, this service automatically provides the IO logging module, which means it automatically writes data per day's audits:
         * this could not be disabled because of its prior potential or idea of providing information when client just crashes.
         */

        public static Task Info(string msg, string type = "def")
        {
            var entry_date
                         = DateTime.Now.ToString("dd/MM/yyyy");

            var ref_path = $"audit-{entry_date}.logs";

            new LogsIO().Init(ref_path);

            switch(type)
            {
                case "def":
                    string def_entry = "[GENERAL/INFO] " + msg + "\n";

                    byte[] def_bytes = Encoding.UTF8.GetBytes(def_entry);

                    new LogsIO().Merge("logs/" + ref_path, def_bytes);

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write(def_entry);

                    Console.ResetColor();

                    break;

                case "cmd":
                    string cmd_entry = "[COMMAND/INFO]" + msg + "\n";

                    byte[] cmd_bytes = Encoding.UTF8.GetBytes(cmd_entry);

                    new LogsIO().Merge("logs/" + ref_path, cmd_bytes);

                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write(cmd_entry);

                    Console.ResetColor();
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Warn(string msg, string type = "def")
        {
            var entry_date
                         = DateTime.Now.ToString("dd/MM/yyyy");

            var ref_path = $"audit-{entry_date}.logs";

            new LogsIO().Init(ref_path);

            switch (type)
            {
                case "def":
                    string def_entry = "[GENERAL/WARN] " + msg + "\n";

                    byte[] def_bytes = Encoding.UTF8.GetBytes(def_entry);

                    new LogsIO().Merge("logs/" + ref_path, def_bytes);

                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.Write(def_entry);

                    Console.ResetColor();

                    break;

                case "cmd":
                    string cmd_entry = "[COMMAND/WARN] " + msg + "\n";

                    byte[] cmd_bytes = Encoding.UTF8.GetBytes(cmd_entry);

                    new LogsIO().Merge("logs/" + ref_path, cmd_bytes);

                    Console.ForegroundColor = ConsoleColor.Yellow;

                    Console.Write(cmd_entry);

                    Console.ResetColor();
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Error(string msg, string type = "def")
        {
            var entry_date
                         = DateTime.Now.ToString("dd/MM/yyyy");

            var ref_path = $"audit-{entry_date}.logs";

            new LogsIO().Init(ref_path);

            switch (type)
            {
                case "def":
                    string def_entry = "[GENERAL/ERROR] " + msg + "\n";

                    byte[] def_bytes = Encoding.UTF8.GetBytes(def_entry);

                    new LogsIO().Merge("logs/" + ref_path, def_bytes);

                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write(def_entry);

                    Console.ResetColor();

                    break;

                case "cmd":
                    string cmd_entry = "[COMMAND/ERROR]" + msg + "\n";

                    byte[] cmd_bytes = Encoding.UTF8.GetBytes(cmd_entry);

                    new LogsIO().Merge("logs/" + ref_path, cmd_bytes);

                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.Write(cmd_entry);

                    Console.ResetColor();
                    break;
            }

            return Task.CompletedTask;
        }

        public static Task Success(string msg, string type = "def")
        {
            var entry_date
                         = DateTime.Now.ToString("dd/MM/yyyy");

            var ref_path = $"audit-{entry_date}.logs";

            new LogsIO().Init(ref_path);

            switch (type)
            {
                case "def":
                    string def_entry = "[GENERAL/SUCCESS] " + msg + "\n";

                    byte[] def_bytes = Encoding.UTF8.GetBytes(def_entry);

                    new LogsIO().Merge("logs/" + ref_path, def_bytes);

                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.Write(def_entry);

                    Console.ResetColor();

                    break;

                case "cmd":
                    string cmd_entry = "[COMMAND/SUCCESS]" + msg + "\n";

                    byte[] cmd_bytes = Encoding.UTF8.GetBytes(cmd_entry);

                    new LogsIO().Merge("logs/" + ref_path, cmd_bytes);

                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.Write(cmd_entry);

                    Console.ResetColor();
                    break;
            }

            return Task.CompletedTask;
        }
    }
}