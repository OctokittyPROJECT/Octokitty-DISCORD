using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctokittyBOT
{
    public static class Config
    {
        /// <summary>
        /// Creates a configuration file for bot's setup and reads it via file's existing
        /// </summary>
        public static void InitCfg()
        {
            if(!File.Exists(".env"))
            {
                using(FileStream stream = File.Create(".env"))
                {
                    string cfgPattern = "DISCORD_TOKEN=\nGITHUB_TOKEN=\nBOT_PREFIX=+\nGUILD=";

                    byte[] patternBytes = Encoding.UTF8.GetBytes(cfgPattern);

                    stream.Write(patternBytes, 0, patternBytes.Length);
                    stream.Close();
                }

                ReadCfg();

                // We are closing an application, because bot won't start without
                // specified context in DOTENV file

                Environment.Exit(0);
            }
            else
                ReadCfg();
        }

        /// <summary>
        /// Reads DOTENV configuration's file and sets environment's variables into app's process
        /// </summary>
        /// <exception cref="FileNotFoundException">Throws when DOTENV file doesn't exist</exception>
        private static void ReadCfg()
        {
            if (!File.Exists(".env"))
                throw new FileNotFoundException("ENV file does not exist! Please, restart an application for its normal setup.");

            string[] lines = File.ReadAllLines(".env");

            foreach(string line in lines)
            {
                string[] variable = line.Split('=');

                Environment.SetEnvironmentVariable(variable[0], variable[1]);
            }
        }
    }
}
