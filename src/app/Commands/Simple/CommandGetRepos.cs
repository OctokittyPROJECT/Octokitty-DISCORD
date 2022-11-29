using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using OctokittyBOT.Modules;

namespace OctokittyBOT.Commands.Simple
{
    /// <summary>
    /// Non-static class of <c>"get-repository"</c> command's instance which returns info about repository
    /// </summary>
    /// 
    public sealed class CommandGetRepos : CommandTemplate
    {
        private new readonly DiscordSocketClient? _client;
        private new readonly CommandService? _service;

        /// <summary>
        /// Constructor of <c>"get-repository"</c> command
        /// </summary>
        /// <param name="provider">
        /// An <see cref="IServiceProvider"/> for current service's instances for bot's command constructor
        /// </param>
        /// 
        public CommandGetRepos(IServiceProvider provider)
        {
            _client = provider.GetService<DiscordSocketClient>();
            _service = provider.GetService<CommandService>();

            PingNullables(_client, _service);
        }

        /// <summary>
        /// Constructor of <c>"get-repository"</c> command
        /// </summary>
        /// <param name="client">
        /// A <see cref="DiscordSocketClient">client</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// <param name="service">
        /// A <see cref="CommandService">command service</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// 
        public CommandGetRepos(DiscordSocketClient? client, CommandService? service)
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
        /// Thrown when there is no any valid options to find the repository.
        /// </exception>
        /// 
        public override async Task Execute(SocketSlashCommand command)
        {
            GitHubClient gitHubClient = GitHubConnector.GenerateClientFromEnv();

            string owner = string.Empty,
                   repos = string.Empty;

            long reposId = -1;

            foreach (var option in command.Data.Options)
            {
                switch (option.Name)
                {
                    case "owner":
                        owner = (string)option.Value;
                        break;
                    case "repository":
                        repos = (string)option.Value;
                        break;
                    case "repository_id":
                        reposId = Convert.ToInt64(option.Value);
                        break;
                    default:
                        continue;
                }
            }

            try
            {
                Repository? repo = null;

                if (owner != string.Empty && repos != string.Empty)
                    repo = await gitHubClient.Repository.Get(owner, repos);
                else if (reposId != -1)
                    repo = await gitHubClient.Repository.Get(reposId);
                else
                    throw new ArgumentException("There is no valid argument to find any repository on GitHub!");

                if (repo == null)
                    throw new KeyNotFoundException("There is no required repository specified by you!");
                else
                {
                    var embed = new EmbedBuilder()
                        .WithTitle(repo.FullName)
                        .WithDescription(repo.Description == null ? "No description provided." : repo.Description)
                        .WithUrl(repo.Url)
                        .WithThumbnailUrl(repo.Owner.AvatarUrl)
                        .WithColor(Color.Blue)
                        .WithTimestamp(DateTimeOffset.Now)
                        .AddField("Pushed at:", $"* {(repo.PushedAt != null ? repo.PushedAt : "No data provided")}", true)
                        .AddField("Updated at:", $"* {repo.UpdatedAt}", true)
                        .AddField("Repository's License:", repo.License?.Name, true)
                        .AddField("Repository's Top Language:", repo.Language != null ? repo.Language : "No currency language.", true)
                        .AddField("Repository's ID:", $"- Git's ID: {repo.Id}\n- Node's ID: {repo.NodeId}")
                        .AddField("Repository's Socials:", $"- Stargazers: {repo.StargazersCount}\n- Watchers: {repo.SubscribersCount}\n- Forks: {repo.ForksCount}")
                        .AddField("Repository's State:", $"- Private: {(repo.Private ? "Yes" : "No")}\n- Fork: {(repo.Fork ? "Yes" : "No")}\n- Archive: {(repo.Archived ? "Yes" : "No")}\n - Template: {(repo.IsTemplate ? "Yes" : "No")}")
                        .WithFooter(new EmbedFooterBuilder()
                                        .WithText($"SSH Size: {repo.Size}")
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
