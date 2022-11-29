using Discord;
using Discord.Commands;
using Discord.WebSocket;
using OctokittyBOT.Coroutines;
using System.Diagnostics;

namespace OctokittyBOT.Commands
{
    /// <summary>
    /// Inheritor and base template for commands instances
    /// </summary>
    /// 
    public abstract class CommandTemplate
    {
        /// <summary>
        /// A _client for bot of type
        /// </summary>
        /// 
        public readonly DiscordSocketClient? _client;

        /// <summary>
        /// A bot's command service which handles commands
        /// </summary>
        /// 
        public readonly CommandService? _service;

        /// <summary>
        /// Task, executing the ref of command instance
        /// </summary>
        /// <param name="command">
        /// A <see cref="SocketSlashCommand"/> instance, which is given by <see cref="SlashCommandBuilder"/> and its handler
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Thrown when command doesn't override <see cref="Execute(SocketSlashCommand)"/> task
        /// </exception>
        /// 
        public virtual Task Execute(SocketSlashCommand command)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pings both <see cref="DiscordSocketClient"/>? and <see cref="CommandService"/>? for current instance of <see cref="SocketSlashCommand"/>
        /// </summary>
        /// 
        public virtual void PingNullables(DiscordSocketClient? client, CommandService? service)
        {
            var stackFrame = new StackFrame(2);
            if (client == null)
#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
                Logger.Error(new ArgumentNullException(nameof(_client), $"Parameter in {stackFrame.GetMethod} of {nameof(_client)} is NULL!"), true);
            if (service == null)
                Logger.Error(new ArgumentNullException(nameof(_service), $"Parameter in {stackFrame.GetMethod} of {nameof(_service)} is NULL!"), true);
#pragma warning restore S3928 // Parameter names used into ArgumentException constructors should match an existing one 
        }

        /// <summary>
        /// Generate an <see cref="Embed"/>[] from an <see cref="Exception"/>
        /// </summary>
        /// <param name="command">
        /// A <see cref="SocketSlashCommand"/> instance, which is given by <see cref="SlashCommandBuilder"/> and its handler
        /// </param>
        /// <param name="exception">
        /// An <see cref="Exception"/> which occured via code's work
        /// </param>
        /// <returns>
        /// An <see cref="Embed"/>[] for Discord from given <paramref name="exception"/>
        /// </returns>
        /// 
        public virtual async Task PingError(SocketSlashCommand command, Exception exception)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle("Unexpected error occured!")
                .WithDescription(exception.Message)
                .WithTimestamp(DateTimeOffset.Now)
                .WithColor(Color.Red)
                .AddField("Exception stacktrace: ", exception.StackTrace, true)
                .AddField("Exception's source: ", exception.Source, true)
                .AddField("Exception's data: ", $"- Keys: {string.Join(':', exception.Data.Keys)}\n- Values: {string.Join(':', exception.Data.Values)}", false);

            await command.RespondAsync(null, new Embed[] { embed.Build() }, false, true);
            Logger.Error(exception.Message, "COMMAND", true);
        }
    }
}