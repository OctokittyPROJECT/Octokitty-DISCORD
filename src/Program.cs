using System;
using System.Text;

using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;

using Newtonsoft.Json;

using Octokitty.Environment;

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

        private async Task Run()
        {
            Console.OutputEncoding = Encoding.Unicode;

            Configuration.Setup();

            var crt_check = Checkout.Init();

            if (!crt_check)
                return;


            auth_token = Configuration.auth_token;
            bot_prefix = Configuration.bot_prefix;

            bot_client = new DiscordSocketClient();

            await bot_client.LoginAsync(TokenType.Bot, auth_token);
            await bot_client.StartAsync();

            await Task.Delay(-1);
        }
    }
}