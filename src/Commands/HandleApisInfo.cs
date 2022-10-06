using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Requester = OctokittyBOT.Requests.RequestGithub;

using Octokit;
using Discord;

namespace OctokittyBOT.Commands
{
    public static class HandleApisInfo
    {
        public static async Task Execute(SocketSlashCommand command)
        {
            var gitClient = Requester.ConnectGitHubClient("Octokitty");

            var obj = new object();
            lock(obj)
            {
                Console.WriteLine(gitClient.Credentials);
            }

            var miscRateLimit = await gitClient.Miscellaneous.GetRateLimits();

            var coreRateLimit = miscRateLimit.Resources.Core;
            var searchRateLimit = miscRateLimit.Resources.Search;

            var coreRequestsPerHour = coreRateLimit.Limit;
            var coreRequestsLeft = coreRateLimit.Remaining;
            var coreRequestsReset = coreRateLimit.Reset;

            var searchRequestsPerHour = searchRateLimit.Limit;
            var searchRequestsLeft = searchRateLimit.Remaining;
            var searchRequestsReset = searchRateLimit.Reset;

            EmbedBuilder embed = new EmbedBuilder()
                                    .WithTitle("APIs status and info")
                                    .AddField(
                                    "**SEARCH-type requests:**",
                                    $"**PER HOUR:** {searchRequestsPerHour}\n**LEFT:** {searchRequestsLeft}\n**RESET:** {searchRequestsReset}",
                                    false)
                                    .AddField(
                                    "**CORE-type requets:**",
                                    $"**PER HOUR:** {coreRequestsPerHour}\n**LEFT:** {coreRequestsLeft}\n**RESET:** {coreRequestsReset}",
                                    false);

            await command.RespondAsync("", new Embed[] { embed.Build() }, false, true);
        }
    }
}
