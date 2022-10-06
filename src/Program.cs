using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using OctokittyBOT.Commands;

namespace OctokittyBOT
{
    public class Program
    {
        private static void Main(string[] args)
                => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commandService;

        private async Task MainAsync()
        {
            Config.InitCfg();

            _client = new DiscordSocketClient();
            _commandService = new CommandService();

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
            await _client.StartAsync();

            _client.Log += Log;
            _commandService.Log += Log;

            _client.Ready += Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            await Task.Delay(-1);
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch(command.Data.Name)
            {
                case "apis-info":
                    await HandleApisInfo.Execute(command);
                    break;
            }
        }

        private async Task Ready()
        {
            ulong id;

            ulong.TryParse(Environment.GetEnvironmentVariable("GUILD"), out id);

            if (id == 0)
                throw new ArgumentException("Failed while trying to parse environment's variable \"GUILD\", please check the app's configuration!");

            var guildCommands = new List<SlashCommandBuilder>()
            {
                new SlashCommandBuilder()
                    .WithName("apis-info")
                    .WithDescription("Gives you an information about Github API's status and bot's condition")
                    .WithDescriptionLocalizations(new Dictionary<string, string>
                    {
                        { "ru", "Возвращает текущую информацию об статусе Github API и жизнеспособности бота" },
                        { "en-GB", "Gives you an information about Github API's status and bot's condition" },
                        { "en-US", "Gives you an information about Github API's status and bot's condition" }
                    })
                    .WithDMPermission(false)
            };

            try
            {
                foreach (var guildCommand in guildCommands)
                    await _client.Rest.CreateGuildCommand(guildCommand.Build(), id);
            }
            catch(HttpException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                Logger.Error(json, "REGISTRY");
            }
        }

        private Task Log(LogMessage arg)
        {
            if (arg.Exception is CommandException cmdException)
            {
                Logger.Error($"Command [{cmdException.Command.Aliases.First()}] failed to execute in {cmdException.Context.Channel}.", "command");
                Logger.Error(cmdException.ToString(), "COMMAND");
            }
            else if(arg.Exception != null)
            {
                Logger.Error(arg.Exception?.ToString(), "GENERAL");
            }
            else
                Logger.Info(arg.Message?.ToString(), "GENERAL");


            return Task.CompletedTask;
        }
    }
}