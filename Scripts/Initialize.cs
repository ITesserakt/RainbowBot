using System;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using Informatics.Scripts.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace Informatics.Scripts {
    public class Initialize {
        private readonly CommandService      _commandSvs;
        private readonly DiscordSocketClient _bot;
        public Initialize(DiscordSocketClient bot = null, CommandService commandSvs = null) {
            _bot = bot               ?? new DiscordSocketClient();
            _commandSvs = commandSvs ?? new CommandService();
        }

        public void CheckSummary() =>
            _commandSvs.Commands.Where(cmd => string.IsNullOrWhiteSpace(cmd.Summary)).ToList()
                       .ForEach(cmd => Console.WriteLine(
                                    $"[Warning] There is no summary for command {cmd.Name} in module {cmd.Module.Name}")
                       );
        

        public IServiceProvider BuildServiceProvider() =>
            new ServiceCollection()
                .AddSingleton(_bot)
                .AddSingleton(_commandSvs)
                .AddSingleton<CommandHandler>()
                .AddSingleton<Rainbow>()
                .AddSingleton<UsersRoles>()
                .AddSingleton<BotPrefixes>()
                .AddSingleton<InitializeBot>()
                .BuildServiceProvider();
    }
}