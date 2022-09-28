namespace Octokitty.Environments
{
    internal static class Checkout
    {
        /*
         * Const-array of usually banned chars in different tokens:
         * the dot is excluded from the list because its used in Discord's API tokens.
         * 
         * For some ideological presenses, this module could not be disabled: if this code-block
         * preventing your bot from setup, ensure:
         * 
         * -    your tokens are written in correct format: no very-smart symbols in them and etc.
         * -    this are tokens for correct APIs.
         * 
         * In case of everything correct, but this module still blocking you out, please, write an issue about it on
         * bot's source code repository.
         */

        static char[] BANNED_CHARS = new char[] { '\'', '\"', ':', ';', '\\', '@', '#', '№', '$', '%', '^', '&', '?', '*', '(', ')', '{', '}', '<', '>', ',', '/', '|', '!', '~', '`', ' ' };

        public static dynamic ChecksOAuth(string unchecked_hash)
        {
            foreach(char banned_symbol in BANNED_CHARS)
            {
                if (unchecked_hash.Contains(banned_symbol))
                {
                    Logger.Warn("Invalid format of OAuth token!");

                    return false;
                }
                else
                    continue;
            }

            return true;
        }
    }
}