namespace Octokitty.Environment
{
    internal static class Checkout
    {

        static char[] BANNED_CHARS = new char[] { '\'', '\"', ':', ';', '\\', '@', '#', '№', '$', '%', '^', '&', '?', '*', '(', ')', '{', '}', '<', '>', ',', '/', '|', '!', '~', '`', ' ' };

        public static dynamic Init()
        {
            string token = Configuration.GetOAuth();

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