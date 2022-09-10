using System;
using System.Text;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;

using Newtonsoft.Json;

using Octokitty.Environments;
using Octokitty.Coroutines;
using System.Reflection;

namespace Octokitty
{
    internal class Program
    {
        string auth_token;
        string bot_prefix;

        ulong host_domain;

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

            var crt_check = Checkout.Init();

            if (!crt_check)
                return;


            auth_token = Configuration.AUTH_TOKEN;
            bot_prefix = Configuration.BOT_PREFIX;

            bot_client = new DiscordSocketClient();

            command_service = new CommandService();

            await bot_client.LoginAsync(TokenType.Bot, auth_token);
            await bot_client.StartAsync();

            bot_client.Log += Log;

            bot_client.Ready += Ready;

            await Task.Delay(-1);
        }

        private Task Ready()
        {
            host_domain = Configuration.HOST_DOMAIN;

            new GuildsIO().Charge(new dynamic[] { });

            var host_guild = bot_client.GetGuild(host_domain);

            return Task.CompletedTask;
        }

        private Task Log(LogMessage entry)
        {
            var ARG = Convert.ToString(entry);

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