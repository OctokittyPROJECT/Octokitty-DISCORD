using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OctokittyBOT.Coroutines;
using OctokittyBOT.Commands.Simple;
using System.Text;
using System.Windows;
using OctokittyBOT.Modules;

namespace OctokittyBOT
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'Type_or_Member'
    public class Program
    {
        public static void Main(string[] args)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'Type_or_Member'
                => new Program().Process(args).GetAwaiter().GetResult();

        #region Main process and handler of bot

        private DiscordSocketClient? _client;
        private CommandService? _service;

        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Constructor for bot's core
        /// </summary>
        public Program()
        {
            _serviceProvider = CreateProvider();
        }

        static IServiceProvider CreateProvider()
        {
            DiscordSocketConfig config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged,
            };

            CommandServiceConfig serviceConfig = new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
                IgnoreExtraArgs = true,
                LogLevel = LogSeverity.Debug
            };

            var collection = new ServiceCollection()
                .AddSingleton(config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(serviceConfig)
                .AddSingleton<CommandService>();

            return collection.BuildServiceProvider();
        }

        /// <summary>
        /// Main handler of bot's processing
        /// </summary>
        public async Task Process(string[] args)
        {
            Config.InitCfg();

            PrepareConsole();

            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

            _service = _serviceProvider.GetRequiredService<CommandService>();

            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BOT_TOKEN"));
            await _client.StartAsync();

            _client.Ready += Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Method for preparing console for bot's processing
        /// </summary>
        private void PrepareConsole()
        {
            Console.Title = $"OctokittyBOT terminal, starting datetime: {DateTimeOffset.Now} (Offset UTC)";

            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
        }
        #endregion

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "apis-info":
                    await new CommandApisInfo(_serviceProvider).Execute(command);
                    break;
                case "get-repository":
                    await new CommandGetRepos(_serviceProvider).Execute(command);
                    break;
                case "get-branch":
                    await new CommandGetBranch(_serviceProvider).Execute(command);
                    break;
                case "get-release":
                    await new CommandGetRelease(_serviceProvider).Execute(command);
                    break;
                case "get-issue":
                    await new CommandGetIssue(_serviceProvider).Execute(command);
                    break;
                default:
                    await command.RespondAsync("An unexpected error via handling command! Bot is not responding!", null, false, true);
                    break;
            }
        }

        private async Task Ready()
        {
            Logger.Info("Bot is ready to work!", null, true);

            var gitClient = GitHubConnector.GenerateClientFromEnv();

            var release = await gitClient.Repository.Release.Get("AlbeeTheLoli", "deltaLauncher", "0.0.0");

            Console.WriteLine(release.Name);

            // Here we can force non-nullable because reader of ENV config already checks on null

            var guild = _client!.GetGuild(ulong.Parse(Environment.GetEnvironmentVariable("GUILD_ID")!));

            SlashCommandBuilder[] guildCommands = new SlashCommandBuilder[]
            {
                new SlashCommandBuilder()
                .WithName("apis-info")
                .WithDescription("Throws out all required information about current APIs state both from Github and Discord")
                .WithDescriptionLocalizations(new Dictionary<string, string>
                {
                    { "en-GB", "Throws out all required information about current APIs state both from Github and Discord" },
                    { "en-US", "Throws out all required information about current APIs state both from Github and Discord" },
                })
                .WithDMPermission(false),
                new SlashCommandBuilder()
                .WithName("get-repository")
                .WithDescription("Gets all required information about given repository via owner/repository's name or ID")
                .WithDescriptionLocalizations(new Dictionary<string, string>
                {
                    { "en-GB", "Gets all required information about given repository via owner/repository's name or ID" },
                    { "en-US", "Gets all required information about given repository via owner/repository's name or ID" },
                })
                .WithDMPermission(false)
                .AddOption("owner", ApplicationCommandOptionType.String, "Repository's owner nickname/login")
                .AddOption("repository", ApplicationCommandOptionType.String, "Repository's name specified by owner")
                .AddOption("repository_id", ApplicationCommandOptionType.Number, "A 32-bit integer value (long) representing an ID of repository"),
                new SlashCommandBuilder()
                .WithName("get-branch")
                .WithDescription("Gets all required information about given repository's branch via owner/repository's name or ID")
                .WithDescriptionLocalizations(new Dictionary<string, string>
                {
                    { "en-GB", "Gets all required information about given repository's branch via owner/repository's name or ID" },
                    { "en-US", "Gets all required information about given repository's branch via owner/repository's name or ID" },
                })
                .WithDMPermission(false)
                .AddOption("owner", ApplicationCommandOptionType.String, "Repository's owner nickname/login")
                .AddOption("repository", ApplicationCommandOptionType.String, "Repository's name specified by owner")
                .AddOption("repository_id", ApplicationCommandOptionType.Number, "A 32-bit integer value (long) representing an ID of repository")
                .AddOption("branch_name", ApplicationCommandOptionType.String, "Branch's name of repository", true),
                new SlashCommandBuilder()
                .WithName("get-release")
                .WithDescription("Gets all required information about given repository's release via owner/repository's name or ID")
                .WithDescriptionLocalizations(new Dictionary<string, string>
                {
                    { "en-GB", "Gets all required information about given repository's release via owner/repository's name or ID" },
                    { "en-US", "Gets all required information about given repository's release via owner/repository's name or ID" },
                })
                .WithDMPermission(false)
                .AddOption("owner", ApplicationCommandOptionType.String, "Repository's owner nickname/login")
                .AddOption("repository", ApplicationCommandOptionType.String, "Repository's name specified by owner")
                .AddOption("repository_id", ApplicationCommandOptionType.Number, "A 32-bit integer value (long) representing an ID of repository")
                .AddOption("tag", ApplicationCommandOptionType.String, "Release's target tag")
                .AddOption("release_id", ApplicationCommandOptionType.Integer, "Release's target ID"),
                new SlashCommandBuilder()
                .WithName("get-issue")
                .WithDescription("Gets all required information about given repository's issue via owner/repository's name or ID")
                .WithDescriptionLocalizations(new Dictionary<string, string>
                {
                    { "en-GB", "Gets all required information about given repository's issue via owner/repository's name or ID" },
                    { "en-US", "Gets all required information about given repository's issue via owner/repository's name or ID" },
                })
                .WithDMPermission(false)
                .AddOption("owner", ApplicationCommandOptionType.String, "Repository's owner nickname/login")
                .AddOption("repository", ApplicationCommandOptionType.String, "Repository's name specified by owner")
                .AddOption("repository_id", ApplicationCommandOptionType.Number, "A 32-bit integer value (long) representing an ID of repository")
                .AddOption("issue_number", ApplicationCommandOptionType.Integer, "ID number of issue in given repos", true)
            };

            foreach (SlashCommandBuilder command in guildCommands)
            {
                try
                {
                    // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
                    await guild.CreateApplicationCommandAsync(command.Build());
                }
                catch (HttpException exception)
                {
                    var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
                    Logger.Error(json, "COMMAND", true);

                    // We don't use Logger.Error(exception) because we don't want to throw an exception, we already got one
                }
            }
        }

        private Task Log(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Logger.Error($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()} failed to execute in {cmdException.Context.Channel}", "CMD", true);
                Logger.Error(cmdException, true);
            }
            else if (message.Exception != null)
            {
                Logger.Error(message.Exception.Message, "MAINTANCE", true);
                Logger.Error(message.Exception, true);
            }
            else if (message.Exception == null && message.Message.ToLower().Contains("exception"))
                Logger.Error(message.Message, null, true);
            else
                Logger.Info(message.Message, null, true);

            return Task.CompletedTask;
        }
    }
}