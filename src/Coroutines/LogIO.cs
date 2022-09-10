using System;
using System.IO;
using System.Text;

namespace Octokitty.Coroutines
{
    internal class LogIO
    {
        public Task Init(string path)
        {
            if (!Directory.Exists("logs/"))
                 Directory.CreateDirectory("logs/");

            if(!File.Exists("logs/" + path))
            {
                using(FileStream stream = File.Create("logs/" + path))
                {
                    string init_typo = "[INIT] " + $"{DateTime.Now.ToString("HH:mm:ss tt")}" + "Initialized this audit journal successfully!" + "\n"
                                     + "[INIT] " + $"{DateTime.Now.ToString("HH:mm:ss tt")}" + "Configuration of this log file are complete." + "\n";

                    byte[] init_bytes = Encoding.UTF8.GetBytes(init_typo);

                    stream.Write(init_bytes, 0, init_bytes.Length);

                    stream.Close();
                }

                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine("[INIT] " + $"{DateTime.Now.ToString("HH:mm:ss tt")}" + "Initialized the audits successfully!");
                Console.WriteLine("[INIT] " + $"{DateTime.Now.ToString("HH:mm:ss tt")}" + "Configuration of log file are ended.");

                Console.ResetColor();
            }

            return Task.CompletedTask;
        }

        public Task Merge(string path, byte[] input)
        {
            string audit_context = File.ReadAllText(path);

            byte[] context_bytes = Encoding.UTF8.GetBytes(audit_context);
            byte[] combined_output 
                                 = context_bytes.Concat(input).ToArray();

            File.WriteAllBytes(path, combined_output);

            return Task.CompletedTask;
        }
    }
}
