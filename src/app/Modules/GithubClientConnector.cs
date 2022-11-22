using Octokit;

namespace OctokittyBOT.Modules
{
    /// <summary>
    /// Class for fast and cozy requester for <see cref="GitHubAppsClient"/>
    /// </summary>
    public static class GitHubConnector
    {
        /// <summary>
        /// Generates a <see cref="GitHubClient"/> from ENV variables
        /// </summary>
        /// <returns>
        /// Returns a <see cref="GitHubClient"/> with <c>token</c> <see cref="Credentials"/> which created from <see cref="Environment">ENV</see>
        /// </returns>
        public static GitHubClient GenerateClientFromEnv()
        {
            // When authenticated, you have 5000 requests per hour available.
            // So this is the recommended approach for interacting with the API.

            string? headerValue = Environment.GetEnvironmentVariable("GITHUB_PRODUCT_HEADER");

            string headerName = (headerValue == null) ? "OctokittyBOT" : headerValue;

            var client = new GitHubClient(new ProductHeaderValue($"{headerName}"));
            var credentials = new Credentials(Environment.GetEnvironmentVariable("GITHUB_TOKEN")!);

            client.Credentials = credentials;

            return client;
        }

        /// <summary>
        /// Generates an unauthorized <see cref="GitHubClient"/>
        /// </summary>
        /// <remarks>
        /// Unauthorized client have lesser rate's limits, but when authenticated, you have 5000 requests per hour available.
        /// </remarks>
        /// <param name="headerValue">
        /// The API will reject you if you don't provide a User-Agent header (more details at <seealso href="https://developer.github.com/v3/#user-agent-required">here</seealso>). This is also to identify applications that are accessing the API and enable GitHub to contact the application author if there are problems.
        /// </param>
        /// <returns>
        /// Returns an unauthorized <see cref="GitHubClient"/> only with <paramref name="headerValue"/>
        /// </returns>
        public static GitHubClient GenerateClient(string headerValue = "OctokittyBOT")
        {
            return new GitHubClient(new ProductHeaderValue(headerValue));
        }

        /// <summary>
        /// Generates an authorized <see cref="GitHubClient"/> with <see cref="Credentials"/> by <paramref name="token"/>
        /// </summary>
        /// <param name="token">
        /// A string representing a PAT (personal access token) with which your application will connect</param>
        /// <param name="headerValue">
        /// The API will reject you if you don't provide a User-Agent header (more details at <seealso href="https://developer.github.com/v3/#user-agent-required">here</seealso>). This is also to identify applications that are accessing the API and enable GitHub to contact the application author if there are problems.
        /// </param>
        /// <returns>
        /// Returns an authorized <see cref="GitHubClient"/> by <see cref="Credentials"/> with <paramref name="token"/>
        /// </returns>
        public static GitHubClient GenerateClient(string token, string headerValue = "OctokittyBOT")
        {
            // When authenticated, you have 5000 requests per hour available.
            // So this is the recommended approach for interacting with the API.

            var client = new GitHubClient(new ProductHeaderValue(headerValue));
            var credentials = new Credentials(token);

            client.Credentials = credentials;

            return client;
        }

        /// <summary>
        /// Generates an authorized <see cref="GitHubClient"/> with <see cref="Credentials"/> by <paramref name="username"/> and <paramref name="password"/>
        /// </summary>
        /// <param name="username">
        /// A login (username) for account to which you want connect the bot
        /// </param>
        /// <param name="password">
        /// A password to given login's credentials to which you want connect the bot
        /// </param>
        /// <param name="headerValue">
        /// The API will reject you if you don't provide a User-Agent header (more details at <seealso href="https://developer.github.com/v3/#user-agent-required">here</seealso>). This is also to identify applications that are accessing the API and enable GitHub to contact the application author if there are problems.
        /// </param>
        /// <returns>
        /// Returns an authorized <see cref="GitHubClient"/> by <see cref="Credentials"/> with <paramref name="username"/> and <paramref name="password"/>
        /// </returns>
        public static GitHubClient GenerateClient(string username, string password, string headerValue = "OctokittyBOT")
        {
            // When authenticated, you have 5000 requests per hour available.
            // So this is the recommended approach for interacting with the API.

            var client = new GitHubClient(new ProductHeaderValue(headerValue));
            var credentials = new Credentials(username, password);

            client.Credentials = credentials;

            return client;
        }

        /// <summary>
        /// Generate a <see cref="GitHubClient"/> with <c>GitHub Enterprise</c>
        /// </summary>
        /// <param name="enterpriseUri">
        /// An <see cref="Uri"/> or URL to your enterprise client
        /// </param>
        /// <param name="headerValue">
        /// The API will reject you if you don't provide a User-Agent header (more details at <seealso href="https://developer.github.com/v3/#user-agent-required">here</seealso>). This is also to identify applications that are accessing the API and enable GitHub to contact the application author if there are problems.
        /// </param>
        /// <example>
        /// <code>
        /// var ghe = new Uri("https://github.myenterprise.com/");
        /// var client = GithubClientConnector.GenerateClient(ghe, "my-enterprise");
        /// </code>
        /// </example>
        /// <returns>
        /// Returns a <see cref="GitHubClient"/> with Enterprise client
        /// </returns>
        public static GitHubClient GenerateClient(Uri enterpriseUri, string headerValue)
        {
            var client = new GitHubClient(new ProductHeaderValue(headerValue), enterpriseUri);

            return client;
        }
    }
}
