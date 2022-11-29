using System.Text;

namespace OctokittyBOT.Coroutines
{
    /// <summary>
    /// Handles any utility task for this bot
    /// </summary>
    /// 
    public static class Utils
    {
        /// <summary>
        /// Creates a config file and exits the environment
        /// </summary>
        /// <param name="path">
        /// A path to ENV config file
        /// </param>
        /// 
        public static void PrepareCfg(string path)
        {
            using (FileStream stream = File.Create(path))
            {
                string contents = "BOT_TOKEN=\nGITHUB_TOKEN=\n# Enter ID of bot's client\nCLIENT_ID=\n# Enter an ID of your dev-server or on which your bot will locally work\nGUILD_ID=";

                byte[] contentsInBytes = Encoding.UTF8.GetBytes(contents);

                stream.Write(contentsInBytes, 0, contentsInBytes.Length);
                stream.Close();
            }

            Logger.Warn("Created ENV config file successfully! Please, close console and type required data!", "CONFIG", false);

            Environment.Exit(0);
        }

        /// <summary>
        /// Works with file system of logger and write data in it
        /// </summary>
        /// <param name="path">
        /// Path to log's file
        /// </param>
        /// <param name="contents">
        /// Contents which will be written into audit
        /// </param>
        /// 
        public static void LogsHandler(string path, string contents)
        {
            byte[] contentsInBytes = Encoding.UTF8.GetBytes($"{contents}\n");

            string[] pathArr = path.Split('/');

            string pathFile = pathArr[pathArr.Length - 1];
            string pathDir = path.Remove(path.IndexOf(pathFile), pathFile.Length);

            if (!Directory.Exists(pathDir))
                Directory.CreateDirectory(pathDir);

            /*
             * We are using appending in file by byte's array because
             * it works faster than default append function and automatically
             * defined encoding (in our case UTF8) into file.
             */

            if (!File.Exists(path))
            {
                using (FileStream stream = File.Create(path))
                {
                    stream.Write(contentsInBytes, 0, contentsInBytes.Length);
                    stream.Close();
                }
            }
            else
            {
                byte[] prevLogs = File.ReadAllBytes(path);

                using (FileStream stream = File.OpenWrite(path))
                {
                    byte[] mergedLogs = prevLogs.Concat(contentsInBytes).ToArray();

                    stream.Write(mergedLogs, 0, mergedLogs.Length);
                    stream.Close();
                }
            }
        }
    }
}