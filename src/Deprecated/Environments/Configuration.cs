using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Octokitty.Coroutines;

namespace Octokitty.Environments
{
    internal static class Configuration
    {
        public static async void Setup()
        {
            if (!File.Exists("cfg-init.json"))
                using (FileStream stream = File.Create("cfg-init.json"))
                {
                    JObject cfg_pattern = new JObject(
                        new JProperty("auth_token", ""),
                        new JProperty("gits_token", ""),
                        new JProperty("bot_prefix", ""),
                        new JProperty("host_domain", new ulong()));

                    byte[] pattern_bytes = Encoding.UTF8.GetBytes(cfg_pattern.ToString());

                    await stream.WriteAsync(pattern_bytes, 0, pattern_bytes.Length);

                    stream.Close();

                    string message = "Config has been created successfully! Ensure entering required context in it!";

                    int exit_code = Encoding.Unicode.GetByteCount(message);

                    Environment.Exit(exit_code);
                }
            else
            {
                JObject cfg = JObject.Parse(File.ReadAllText("cfg-init.json"));

                Environment.SetEnvironmentVariable("AUTH_TOKEN", cfg.GetValue("auth_token").Value<string>());
                Environment.SetEnvironmentVariable("GITS_TOKEN", cfg.GetValue("gits_token").Value<string>());

                Environment.SetEnvironmentVariable("BOT_PREFIX", cfg.GetValue("bot_prefix").Value<string>());

                Environment.SetEnvironmentVariable("HOST_DOMAIN", cfg.GetValue("host_domain").Value<string>());
            }
        }

        public static void CheckAudit()
        {
            var audit_date
                         = DateTime.Now.ToString("dd/MM/yyyy");

            var audit_path = $"audit-{audit_date}.logs";

            if (File.Exists("logs/" + audit_path))
            {
                /*
                 * Every block of checkout module initialized even before far away when logging service and its IO servitors will be used,
                 * thats why as in the IO servitors we can't use ready-to-use logging service: it will cause an exception of RAM's overflow.
                 */

                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine("[INIT] " + $"{DateTime.Now.ToString("HH:mm:ss tt")}" + "Audit for current date is available at the moment.");
                Console.WriteLine("[INIT] " + $"{DateTime.Now.ToString("HH:mm:ss tt")}" + "Using audit file for current date to continue log.");

                Console.ResetColor();

                string audit_entry = "[INIT] " + $"{DateTime.Now.ToString("HH:mm:ss tt")}" + "Audit for current date is available at the moment." + "\n"
                                   + "[INIT] " + $"{DateTime.Now.ToString("HH:mm:ss tt")}" + "Using audit file for current date to continue log.";

                byte[] audit_bytes = Encoding.UTF8.GetBytes(audit_entry);

                new LogsIO().Merge("logs/" + audit_path, audit_bytes);
            }
        }
    }
}