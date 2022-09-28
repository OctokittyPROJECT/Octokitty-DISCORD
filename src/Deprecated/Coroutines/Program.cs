using System;
using System.Text;
using System.Linq;
using System.Reflection;

using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;

using Newtonsoft.Json;

using Octokitty.Environments;
using Octokitty.Coroutines;

namespace Octokitty.Deprecated.Coroutines
{
    internal class Program
    {
        private static Task Main(string[] args)
            => new Program().Run();

        private DiscordSocketClient bot_client;
        private CommandService command_service;

        private IServiceProvider _provider_srv;

        private async Task Run()
        {
            Console.OutputEncoding = Encoding.Unicode;

            Configuration.Setup();
            Configuration.CheckAudit();

            /*
             * Reading from environment OAuth tokens for their char-checking:
             * function would return respective TRUE or FALSE depending on their contents.
             */

            string auth_token = Environment.GetEnvironmentVariable("AUTH_TOKEN");
            string gthb_token = Environment.GetEnvironmentVariable("GITS_TOKEN");

            var crt_check = Checkout.ChecksOAuth(auth_token);
            var grt_check = Checkout.ChecksOAuth(gthb_token);

            if (!crt_check || !grt_check)
                return;

            bot_client = new DiscordSocketClient();
            command_service = new CommandService();

            await bot_client.LoginAsync(TokenType.Bot, auth_token);
            await bot_client.StartAsync();

            bot_client.Log += Log;

            bot_client.Ready += Ready;

            await PostInit();

            await Task.Delay(-1);
        }

        private async Task PostInit()
        {
            bot_client.MessageReceived += Receive;

            await command_service.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                                  services: null);
        }

        private async Task Receive(SocketMessage skt_message)
        {
            if (skt_message.Author.IsBot) return;
            else
                return;
        }

        private Task Ready()
        {
            var host_domain = ulong.Parse(Environment.GetEnvironmentVariable("HOST_DOMAIN"));

            new GuildsIO().Charge(new dynamic[] { });

            var host_guild = bot_client.GetGuild(host_domain);

            var commands = new List<SlashCommandBuilder>()
            {
                new SlashCommandBuilder()
                    .WithName("NULLABLE_EX")
                    .WithDescription("EXEC")
                    .WithDMPermission(false),
            };

            return Task.CompletedTask;
        }

        private Task Log(LogMessage entry)
        {
            var ARG = Convert.ToString(entry);

            /*
             * API's replies with JSON-alike look are converted into default string object,
             * then we are checking on exception keyword and implementing specified logging service.
             */

            if (ARG.Contains("Exception"))
            {
                Logger.Error(ARG);
            }
            else
                Logger.Info(ARG);

            return Task.CompletedTask;
        }
    }
}