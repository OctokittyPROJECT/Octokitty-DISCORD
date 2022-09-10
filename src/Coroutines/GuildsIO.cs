using System;
using System.IO;
using System.Text;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Octokitty.Environments;

namespace Octokitty.Coroutines
{
    internal class GuildsIO
    {
        public async void Charge(dynamic[] args)
        {
            var host_id = Convert.ToString(Configuration.HOST_DOMAIN);

            if(!File.Exists("guilds-data.json"))
            {
                using(FileStream stream = File.Create("guilds-data.json"))
                {

                    JObject pattern_data = new JObject(
                        new JProperty(host_id, new JObject(
                            new JProperty("deprecated", new bool()),
                            new JProperty("requesting", new bool()))));

                    byte[] bytes_pttrn = Encoding.UTF8.GetBytes(pattern_data.ToString());

                    await stream.WriteAsync(bytes_pttrn, 0, bytes_pttrn.Length);

                    stream.Close();

                    string message = $" {DateTime.Now.ToString("HH:mm:ss tt")}" + "Charged JSON format with host-domain server!";
                    string deploys = $" {DateTime.Now.ToString("HH:mm:ss tt")}" + "Ensure writing an ID of your main server on which bot is going through maintance.";

                    Logger.Success(message);
                    Logger.Success(deploys);

                    string auditor = "JSON has been created successfully! Ensure entering required context in it!";

                    int exit_code = Encoding.Unicode.GetByteCount(auditor);

                    Environment.Exit(exit_code);
                }
            }
        }
    }
}
