using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using OctokittyBOT.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctokittyBOT.Commands.Simple
{
    /// <summary>
    /// Non-static class of <c>"get-issue"</c> command's instance which returns info about repository's issue
    /// </summary>
    /// 
    public sealed class CommandGetIssue : CommandTemplate
    {
        private new readonly DiscordSocketClient? _client;
        private new readonly CommandService? _service;

        /// <summary>
        /// Constructor of <c>"get-issue"</c> command
        /// </summary>
        /// <param name="provider">
        /// An <see cref="IServiceProvider"/> for current service's instances for bot's command constructor
        /// </param>
        /// 
        public CommandGetIssue(IServiceProvider provider)
        {
            _client = provider.GetService<DiscordSocketClient>();
            _service = provider.GetService<CommandService>();

            PingNullables(_client, _service);
        }

        /// <summary>
        /// Constructor of <c>"get-issue"</c> command
        /// </summary>
        /// <param name="client">
        /// A <see cref="DiscordSocketClient">client</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// <param name="service">
        /// A <see cref="CommandService">command service</see> of Discord bot already authorized in <see cref="Program"/>
        /// </param>
        /// 
        public CommandGetIssue(DiscordSocketClient? client, CommandService? service)
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

            int issueNumber = -1;

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
                    case "issue_number":
                        issueNumber = Convert.ToInt32(option.Value);
                        break;
                    default:
                        continue;
                }
            }

            try
            {
                Issue issue;
                Repository? repo;

                if(owner != string.Empty && repos != string.Empty)
                {
                    repo = await gitHubClient.Repository.Get(owner, repos);
                    issue = await gitHubClient.Issue.Get(owner, repos, issueNumber);
                }
                else if(reposId != -1)
                {
                    repo = await gitHubClient.Repository.Get(reposId);
                    issue = await gitHubClient.Issue.Get(reposId, issueNumber);
                }
                else
                    throw new ArgumentException("There is no valid argument to find any issues on repository in GitHub!");

                if(issue == null || repo == null)
                    throw new KeyNotFoundException("There is no required issue in repository specified by you!");
                else
                {
                    List<string> labelsNames = new(),
                                 assigneesNames = new();

                    foreach (var label in issue.Labels)
                        labelsNames.Add(label.Name);
                    foreach (var assignee in issue.Assignees)
                        assigneesNames.Add(assignee.Name);

                    var embed = new EmbedBuilder()
                            .WithTitle(issue.Title)
                            .WithDescription(issue.Body)
                            .WithUrl(issue.Url)
                            .WithThumbnailUrl(issue.User.AvatarUrl)
                            .WithColor(Color.Blue)
                            .WithTimestamp(DateTimeOffset.Now)
                            .AddField("Issue's Number:", issue.Number, true)
                            .AddField("Issue's State:", issue.State.StringValue, true)
                            .AddField("Created at:", issue.CreatedAt.ToString(), true)
                            .AddField("Labels:", labelsNames.Count < 1 ? "No labels provided." : string.Join("/", labelsNames), true)
                            .AddField("Assignees:", assigneesNames.Count < 1 ? "No assignees provided." : string.Join("/", assigneesNames), true)
                            .AddField("Issue's ID data:", $"- ID: {(issue.Id == null ? "No ID provided." : issue.Id)}\n- NodeID: {(issue.NodeId == null ? "No NodeID provided." : issue.NodeId)}", false)
                            //.AddField("Issue's pull request:", $"{(issue.PullRequest == null ? "No PR provided." : $"- PR ID: {issue.PullRequest.Id}\n- URL: {issue.PullRequest.Url}")}", false)
                            //.AddField("Issue's milestone:", $"{(issue.Milestone == null ? "No milestone provided." : $"- Milestone ID: {issue.Milestone.Id}\n- URL: {issue.Milestone.Url}")}", false)
                            //.AddField("URLs:", $"- Events URL: {issue.EventsUrl}\n- Comments URL: {issue.CommentsUrl}")
                            .WithFooter(new EmbedFooterBuilder()
                                            .WithText($"Issue's repository ID: {(issue.ClosedAt == null ? "Not closed." : issue.ClosedAt)}")
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
