using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using OctokittyBOT.Coroutines;
using OctokittyBOT.Modules;
using System.Diagnostics;

namespace OctokittyBOT.Commands
{
    /// <summary>
    /// Non-static class of <c>"apis-info"</c> command which returns info about current APIs status
    /// </summary>
    public sealed class CommandApisInfo : CommandTemplate
    {
        private readonly DiscordSocketClient? _client;
        private readonly CommandService? _service;

        /// <summary>
        /// Constructor of <c>"apis-info"</c> command
        /// </summary>
        /// <param name="provider">
        /// An <see cref="IServiceProvider"/> for current service's instances for bot's command constructor
        /// </param>
        public CommandApisInfo(IServiceProvider provider)
        {
            this._client = provider.GetService<DiscordSocketClient>();
            this._service = provider.GetService<CommandService>();

            this.PingNullables(this._client, this._service);
        }

        /// <summary>
        /// Constructor of <c>"apis-info"</c> command
        /// </summary>
        /// <param name="client">
        /// A <see cref="DiscordSocketClient">client</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// <param name="service">
        /// A <see cref="CommandService">command service</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        public CommandApisInfo(DiscordSocketClient? client, CommandService? service)
        {
            this._client = client;
            this._service = service;

            this.PingNullables(this._client, this._service);
        }

        /// <summary>
        /// Process to execute main block of code of <paramref name="command"/>
        /// </summary>
        /// <param name="command">
        /// A <see cref="SocketSlashCommand"/> which was started by user and handled into this code-block
        /// </param>
        public override async Task Execute(SocketSlashCommand command)
        {
            GitHubClient gitHubClient = GitHubConnector.GenerateClientFromEnv();

            try
            {
                var rateLimits = await gitHubClient.RateLimit.GetRateLimits();

                //  The "core" object provides your rate limit status except for the Search API.
                var coreRateLimit = rateLimits.Resources.Core;

                var coreRatesPerHour = coreRateLimit.Limit;
                var coreRatesLeft = coreRateLimit.Remaining;
                var coreRatesResetTime = coreRateLimit.Reset; // UTC time

                // the "search" object provides your rate limit status for the Search API.
                var searchRateLimit = rateLimits.Resources.Search;

                var searchRatesPerHour = searchRateLimit.Limit;
                var searchRatesLeft = searchRateLimit.Remaining;
                var searchRatesResetTime = searchRateLimit.Reset; // UTC time

                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle("APIs status")
                    .WithColor(Color.Green)
                    .WithTimestamp(DateTimeOffset.Now)
                    .WithThumbnailUrl(_client!.CurrentUser.GetAvatarUrl())
                    .AddField("Bot current status:", _client.ConnectionState.ToString(), true)
                    .AddField("Bot current latency:", _client.Latency.ToString(), true)
                    .AddField("Github API core rates:", $"- Limits (per hour): {coreRatesPerHour}\n- Remaining: {coreRatesLeft}\n- Reset time (UTC): {coreRatesResetTime}", false)
                    .AddField("Github API search rates:", $"- Limits (per hour): {searchRatesPerHour}\n- Remaining: {searchRatesLeft}\n- Reset time (UTC): {searchRatesResetTime}", false);

                await command.RespondAsync(null, new Embed[] { embed.Build() }, false, true);
            }
            catch (HttpException ex)
            {
                await PingError(command, ex); // Pinging and responding an exception into logs (console) and to user
            }
        }
    }
}