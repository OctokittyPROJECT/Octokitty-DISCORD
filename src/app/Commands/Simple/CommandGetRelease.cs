using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using OctokittyBOT.Modules;

namespace OctokittyBOT.Commands.Simple
{
    /// <summary>
    /// Non-static class of <c>"get-release"</c> command's instance which returns info about repository's release
    /// </summary>
    /// 
    public sealed class CommandGetRelease : CommandTemplate
    {
        private new readonly DiscordSocketClient? _client;
        private new readonly CommandService? _service;

        /// <summary>
        /// Constructor of <c>"get-release"</c> command
        /// </summary>
        /// <param name="provider">
        /// An <see cref="IServiceProvider"/> for current service's instances for bot's command constructor
        /// </param>
        /// 
        public CommandGetRelease(IServiceProvider provider)
        {
            _client = provider.GetService<DiscordSocketClient>();
            _service = provider.GetService<CommandService>();

            PingNullables(_client, _service);
        }

        /// <summary>
        /// Constructor of <c>"get-release"</c> command
        /// </summary>
        /// <param name="client">
        /// A <see cref="DiscordSocketClient">client</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// <param name="service">
        /// A <see cref="CommandService">command service</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// 
        public CommandGetRelease(DiscordSocketClient? client, CommandService? service)
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

            string tag = string.Empty;

            long reposId = -1;
            int releaseId = -1;

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
                    case "release_id":
                        releaseId = Convert.ToInt32(option.Value)!;
                        break;
                    case "tag":
                        tag = Convert.ToString(option.Value)!;
                        break;
                    default:
                        continue;
                }
            }

            try
            {
                Release release;
                Repository? repo;

                if (owner != string.Empty && repos != string.Empty)
                {
                    repo = await gitHubClient.Repository.Get(owner, repos);

                    if (tag != string.Empty)
                        release = await gitHubClient.Repository.Release.Get(owner, repos, tag);
                    else
                        release = await gitHubClient.Repository.Release.Get(owner, repos, releaseId);
                }
                else if (reposId != -1)
                {
                    repo = await gitHubClient.Repository.Get(reposId);

                    if (tag != string.Empty)
                        release = await gitHubClient.Repository.Release.Get(reposId, tag);
                    else
                        release = await gitHubClient.Repository.Release.Get(reposId, releaseId);
                }
                else
                    throw new ArgumentException("There is no valid argument to find any releases on repository in GitHub!");

                if (release == null)
                    throw new KeyNotFoundException("There is no required release in repository specified by you!");
                else
                {
                    var embed = new EmbedBuilder()
                            .WithTitle(release.Name == null ? release.TagName : release.Name)
                            .WithDescription(release.Body == null ? "No release's body provided." : release.Body)
                            .WithUrl(release.Url)
                            .WithThumbnailUrl(repo.Owner.AvatarUrl)
                            .WithColor(Color.Blue)
                            .WithTimestamp(DateTimeOffset.Now)
                            .AddField("Created at:", release.CreatedAt.ToString(), true)
                            .AddField("Tag name:", release.TagName == null ? "No tag's name provided." : release.TagName, true)
                            .AddField("Target commitish:", release.TargetCommitish == null ? "No target provided." : release.TargetCommitish, true)
                            .AddField("Release statuses:", $"- Is draft? {(release.Draft ? "Yes" : "No")}\n- Is pre-release? {(release.Prerelease ? "Yes" : "No")}", false)
                            .AddField("Release's IDs:", $"- ID: {release.Id}\n- NodeID: {(release.NodeId == null ? "No NodeID provided." : release.NodeId)}", false)
                            .AddField("URLs:", $"- Assets URL: {(release.AssetsUrl == null ? "No assets URL provided." : release.AssetsUrl)}\n- Zipball URL: {(release.ZipballUrl == null ? "No zipball URL provided." : release.ZipballUrl)}", false)
                            .WithFooter(new EmbedFooterBuilder()
                                            .WithText($"Release's author: {release.Author.Login}")
                                            .WithIconUrl(_client!.CurrentUser.GetAvatarUrl()));

                    await command.RespondAsync(null, new Embed[] { embed.Build() }, false, true);
                }
            } 
            catch(Exception ex)
            {
                await PingError(command, ex); // Pinging and responding an exception into logs (console) and to user
            }
        }
    }
}
