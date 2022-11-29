using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using OctokittyBOT.Modules;

namespace OctokittyBOT.Commands.Simple
{
    /// <summary>
    /// Non-static class of <c>"get-branch"</c> command's instance which returns info about repository's branch
    /// </summary>
    /// 
    public sealed class CommandGetBranch : CommandTemplate
    {
        private new readonly DiscordSocketClient? _client;
        private new readonly CommandService? _service;

        /// <summary>
        /// Constructor of <c>"get-branch"</c> command
        /// </summary>
        /// <param name="provider">
        /// An <see cref="IServiceProvider"/> for current service's instances for bot's command constructor
        /// </param>
        /// 
        public CommandGetBranch(IServiceProvider provider)
        {
            _client = provider.GetService<DiscordSocketClient>();
            _service = provider.GetService<CommandService>();

            PingNullables(_client, _service);
        }

        /// <summary>
        /// Constructor of <c>"get-branch"</c> command
        /// </summary>
        /// <param name="client">
        /// A <see cref="DiscordSocketClient">client</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// <param name="service">
        /// A <see cref="CommandService">command service</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// 
        public CommandGetBranch(DiscordSocketClient? client, CommandService? service)
        {
            _client = client;
            _service = service;

            PingNullables(_client, _service);
        }

        /// <summary>
        /// Process to execute main block of code of <paramref name="command"/>
        /// </summary>
        /// <param name="command">
        /// A <see cref="SocketSlashCommand"/> which was started by user and handled into this code-block
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when there is no any valid options to find the target.
        /// </exception>
        /// 
        public override async Task Execute(SocketSlashCommand command)
        {
            GitHubClient gitHubClient = GitHubConnector.GenerateClientFromEnv();

            string owner = string.Empty,
                   repos = string.Empty,
                   branchName = "main"; // We are ignoring old-format "master" main because there more a repositories with "main" branch

            long reposId = -1;

            foreach (var option in command.Data.Options)
            {
                switch (option.Name)
                {
                    case "owner":
                        owner = Convert.ToString(option.Value)!;
                        break;
                    case "repository":
                        repos = Convert.ToString(option.Value)!;
                        break;
                    case "repository_id":
                        reposId = Convert.ToInt64(option.Value);
                        break;
                    case "branch_name":
                        branchName = Convert.ToString(option.Value)!;
                        break;
                    default:
                        continue;
                }
            }

            try
            {
                Branch branch;
                Repository? repo;

                if (owner != string.Empty && repos != string.Empty)
                {
                    branch = await gitHubClient.Repository.Branch.Get(owner, repos, branchName);
                    repo = await gitHubClient.Repository.Get(owner, repos);
                }
                else if (reposId != -1)
                {
                    branch = await gitHubClient.Repository.Branch.Get(reposId, branchName);
                    repo = await gitHubClient.Repository.Get(reposId);
                }
                else
                    throw new ArgumentException("There is no valid argument to find any branches on repository in GitHub!");

                if (branch == null)
                    throw new KeyNotFoundException("There is no required branch in repository specified by you!");
                else
                {
                    var embed = new EmbedBuilder()
                            .WithTitle($"{branch.Name} at {repo.FullName}")
                            .WithDescription(repo.Description == null ? "No description provided." : repo.Description)
                            .WithUrl($"{repo.Url}/{branch.Name}")
                            .WithThumbnailUrl(repo.Owner.AvatarUrl)
                            .WithColor(Color.Blue)
                            .WithTimestamp(DateTimeOffset.Now)
                            .AddField("Branch's Name: ", branch.Name, true)
                            .AddField("Is branch protected?", branch.Protected ? "Yes" : "No", true)
                            .AddField("Branch's commit REF: ", branch.Commit.Ref == null ? "No REF provided." : branch.Commit.Ref, true)
                            .AddField("Branch's commit SHA: ", branch.Commit.Sha == null ? "No SHA provided." : branch.Commit.Sha, false)
                            .AddField("Branch's commit NodeID: ", branch.Commit.NodeId == null ? "No node-ID provided." : branch.Commit.NodeId, false)
                            .WithFooter(new EmbedFooterBuilder()
                                            .WithText($"Last update: {repo.UpdatedAt}")
                                            .WithIconUrl(_client!.CurrentUser.GetAvatarUrl()));

                    await command.RespondAsync(null, new Embed[] { embed.Build() }, false, true);
                }
            }
            catch (Exception ex)
            {
                await PingError(command, ex); // Pinging and responding an exception into logs (console) and to user
            }
        }
    }
}
