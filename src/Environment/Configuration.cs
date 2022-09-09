using System;
using System.IO;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Octokitty.Environment
{
    internal static class Configuration
    {
        public static string auth_token { get { return auth_token; } set { auth_token = value; } }
        public static string bot_prefix { get { return bot_prefix; } set { bot_prefix = value; } }

        public static ulong host_domain { get { return host_domain; } set { host_domain = value; } }

        public static async void Setup()
        {
            if (!File.Exists("config.json"))
                using (FileStream stream = File.Create("config.json"))
                {
                    JObject cfg_pattern = new JObject(
                        new JProperty("auth_token", ""),
                        new JProperty("bot_prefix", ""),
                        new JProperty("host_domain", ""));

                    byte[] pattern_bytes = Encoding.UTF8.GetBytes(cfg_pattern.ToString());

                    await stream.WriteAsync(pattern_bytes);

                    stream.Close();
                }
            else
            {
                JObject cfg = JObject.Parse(File.ReadAllText("config.json"));

                auth_token = cfg.GetValue("auth_token").Value<string>();
                bot_prefix = cfg.GetValue("bot_prefix").Value<string>();

                host_domain = cfg.GetValue("host_domain").Value<ulong>();
            }
        }
    }
}