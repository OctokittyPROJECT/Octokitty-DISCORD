namespace Octokitty.Environments
{
    internal static class Checkout
    {

        static char[] BANNED_CHARS = new char[] { '\'', '\"', ':', ';', '\\', '@', '#', '№', '$', '%', '^', '&', '?', '*', '(', ')', '{', '}', '<', '>', ',', '/', '|', '!', '~', '`', ' ' };

        public static dynamic Init()
        {
            string token = Configuration.AUTH_TOKEN;

            foreach (char banned_symbol in BANNED_CHARS)
            {
                if (token.Contains(banned_symbol))
                {
                    Logger.Warn("Invalid format of bot's OAuth token!");

                    return false;
                }
                else
                    continue;
            }

            return true;
        }
    }
}