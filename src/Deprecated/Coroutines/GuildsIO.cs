using System;
using System.IO;
using System.Text;
using System.Reflection;

using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Octokitty.Environments;

namespace Octokitty.Coroutines
{
    internal class GuildsIO
    {
        List<int> LIST = new();
        public Task Charge(dynamic[] args)
        {
            /*
             * Environment variable is actually represents a server of developer or maintaner of the bot,
             * thats because in even first charge of this JSON configuration of individual settings for the guilds we
             * are declaring it in first order.
             */

            string host_id = Environment.GetEnvironmentVariable("HOST_DOMAIN");

            if(!File.Exists("guilds-data.json"))
            {
                using(FileStream stream = File.Create("guilds-data.json"))
                {
                    JArray pattern_data = new JArray()
                    {
                        new JObject(
                            new JProperty("host_const", host_id),
                            new JProperty("deprecated", new bool()),
                            new JProperty("requesting", new bool()))
                    };

                    byte[] bytes_pttrn = Encoding.UTF8.GetBytes(pattern_data.ToString());

                    stream.WriteAsync(bytes_pttrn, 0, bytes_pttrn.Length);

                    stream.Close();

                    string message = $" {DateTime.Now.ToString("HH:mm:ss tt")}" + "Charged JSON format with host-domain server!";
                    string deploys = $" {DateTime.Now.ToString("HH:mm:ss tt")}" + "Ensure writing an ID of your main server on which bot is going through maintance.";

                    Logger.Success(message);
                    Logger.Success(deploys);

                    /*
                     * Closing the application's console because its NECESSARY for the maintaner of bot to see specified args.
                     * More about them in different codes commentaries either documentation for this bot.
                     */

                    string auditor = "JSON has been created successfully! Ensure entering required context in it!";

                    int exit_code = Encoding.Unicode.GetByteCount(auditor);

                    Environment.Exit(exit_code);
                }
            }

            return Task.CompletedTask;
        }

        public Task Implement(dynamic[] args)
        {
            /*
             * Even if we allow user to throw any type of variable, we need to check the exact configuration for JSON:
             * we are using dynamic, because we append any context in JSON that is required.
             * 
             * Also, this few of one methods that is actually throws an exceptions: this was made up because its a very
             * risky method for bot's status.
             */

            if (args.Length == 0) 
                throw new ArgumentException();
            if (args.Length >= 4)
                throw new ArgumentOutOfRangeException();

            if (!File.Exists("guilds-data.json"))
                throw new FileNotFoundException();

            string guilds_json = File.ReadAllText("guilds-data.json");

            List<object> collections = JsonConvert.DeserializeObject<List<object>>(guilds_json);

            var collection_enums = new 
            { 
                id = args[0], 
                deprecated = args[1], 
                requesting = args[2],
            };

            /*
             * Arguable string that makes code a little safer: throwing exception when JSON deseriaizing failed.
             */

            if (collections != null)
                collections.Add(collection_enums);
            else
                throw new ArgumentNullException();

            JArray json_guilds = JArray.FromObject(collections);

            byte[] guilds_byte = Encoding.UTF8.GetBytes(json_guilds.ToString());

            new LogsIO().Merge("guilds-data.json", guilds_byte);

            string deploys = $" {DateTime.Now.ToString("HH:mm:ss tt")}" + "Implemented guild context in specified JSON file! Be ensure about riskness of this operation!";
            string message = $" {DateTime.Now.ToString("HH:mm:ss tt")}" + "Creation of individual configuration ended: if you want to turn some unsupported funcs, use it!";

            Logger.Success(deploys);
            Logger.Success(message);

            return Task.CompletedTask;
        }
    }
}
