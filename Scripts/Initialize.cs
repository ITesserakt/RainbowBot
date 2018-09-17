using System;
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

        public IServiceProvider BuildServiceProvider() =>
            new ServiceCollection()
                .AddSingleton(_bot)
                .AddSingleton(_commandSvs)
                .AddSingleton<Data>()
                .AddSingleton<InitializeBot>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Rainbow>()
                .AddSingleton<UsersRoles>()
                .AddSingleton<AdminsModule>()
                .AddSingleton<BotPrefixes>()
                .BuildServiceProvider();
    }
}