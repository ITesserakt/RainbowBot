using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Informatics.Scripts {
    public class CommandHandler {
        private readonly DiscordSocketClient _bot;
        private readonly CommandService      _command;
        private readonly BotPrefixes         _prefixes;
        private readonly IServiceProvider    _services;

        public CommandHandler(DiscordSocketClient bot, CommandService commands, IServiceProvider services, BotPrefixes prefixes) {
            _bot = bot;
            _command = commands;
            _prefixes = prefixes;
            _services = services;
        }

        public async Task SetupAsync() {
            await _command.AddModulesAsync(Assembly.GetEntryAssembly());
            _bot.MessageReceived += HandleCommandAsync;

            _command.CommandExecuted += OnCommandExecutedAsync;
            _command.Log += LogAsync;
        }

        private Task LogAsync(LogMessage log) {
            if (log.Exception is CommandException ex)
                Console.WriteLine($"{ex.Context.User} failed to execute command '{ex.Command.Name}'\n" +
                                  $"{ex}");
            
            return Task.CompletedTask;
        }

        private static Task OnCommandExecutedAsync(CommandInfo command, ICommandContext context, IResult result) {
            Console.WriteLine($"[{context.Guild.Name}] {context.User}: {context.Message.Content} -> {Success()}");

            string Success() => result.IsSuccess ? "success" : "fail";
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage arg) {
            if (!(arg is SocketUserMessage message)) return;

            var argPos = 0;
            var context = new SocketCommandContext(_bot, message);
            if(context.Guild == null ) {
                return;
            }

            if (!message.HasCharPrefix(_prefixes[context.Guild], ref argPos) ||
                message.HasMentionPrefix(_bot.CurrentUser, ref argPos)) return;

            var result = await _command.ExecuteAsync(context, argPos, _services);
#if DEBUG
            if (!result.IsSuccess && result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand)
                await context.Channel.SendMessageAsync($"Ошибка: {result.Error.Value};\n{result.ErrorReason}");
#endif
        }
    }
}