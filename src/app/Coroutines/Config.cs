namespace OctokittyBOT.Coroutines
{
    /// <summary>
    /// Configuration module for bot
    /// </summary>
    /// 
    public static class Config
    {
        /// <summary>
        /// Initializes and first-step reads vars from config path
        /// </summary>
        /// <param name="path">
        /// A path to ENV config file
        /// </param>
        /// <exception cref="ArgumentException">
        /// Exception will be thrown when one of the variables in ENV config file are not defined
        /// </exception>
        /// 
        public static void InitCfg(string path = ".env")
        {
            if (!File.Exists(path))
                Utils.PrepareCfg(path);
            else
            {
                string[] cfgLines = File.ReadAllLines(path);

                foreach (string cfgParam in cfgLines)
                {
                    if (cfgParam.StartsWith("#"))
                        continue;

                    string[] envVariable = cfgParam.Split('=');

                    if (envVariable[1] == null || envVariable[1] == "")
                    {
                        var exp = new ArgumentException("One of the environment varibales are null!");

                        Logger.Error(exp);
                    }

                    Environment.SetEnvironmentVariable(envVariable[0], envVariable[1]);
                }

                Environment.SetEnvironmentVariable("PATH_TO_CFG", path);

                Logger.Success("Read all environment's variables successfully! Config stage is over, bot is ready to work!", "CONFIG", false);
            }
        }

        /// <summary>
        /// Overwrite param in ENV config file
        /// </summary>
        /// <param name="key">
        /// Key of the value which you want to overwrite
        /// </param>
        /// <param name="value">
        /// A string value which on you want to overwrite
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// If program can't find ENV config path which was when bot started - throws exception
        /// </exception>
        /// 
        public static void WriteCfg(string key, string value = "")
        {
            string path = Environment.GetEnvironmentVariable("PATH_TO_CFG")!;

            if (!File.Exists(path))
            {
                var exp = new FileNotFoundException("File of initialized ENV config was not found! Bot must be in critical state!");

                Logger.Error(exp);
            }
            else
            {
                string[] cfgParams = File.ReadAllLines(path);

                for (int i = 0; i < cfgParams.Length; i++)
                {
                    if (cfgParams[i].StartsWith("#"))
                        continue;

                    if (cfgParams[i].StartsWith(key))
                        cfgParams[i] = $"{key}={value}";
                }
            }
        }
    }
}