using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Informatics.Scripts.Modules;
using Informatics.Scripts.Utilites;
using log4net;
using Microsoft.Extensions.DependencyInjection;

namespace Informatics.Scripts.Core {
    public class Initialize {
        private readonly MuteManager _muteManager;
        private readonly AdminsModule _admins;
        public static readonly ILog Log = LogManager.GetLogger(typeof(Initialize));
        private readonly CommandService _commandSvs;
        private readonly DiscordSocketClient _bot;

        public Initialize(DiscordSocketClient bot = null, CommandService commandSvs = null,
                          MuteManager muteManager = null, AdminsModule admins = null) {
            _muteManager = muteManager;
            _admins = admins;
            _bot = bot ?? new DiscordSocketClient();
            _commandSvs = commandSvs ?? new CommandService();
            _muteManager = muteManager ?? new MuteManager(_bot);
            _admins = admins ?? new AdminsModule(_muteManager);
        }

        public void CheckSummary() =>
            _commandSvs.Commands
                       .Where(cmd => string.IsNullOrWhiteSpace(cmd.Summary)).ToList()
                       .ForEach(cmd => Log.Warn(
                                    $"There is no summary for command '{cmd.Name}' in module '{cmd.Module.Name}'")
                       );

        public async Task CheckMuted() {
            var muted = _muteManager.Check();
            foreach (var (user, (isDone, duration)) in muted) {
                if (!isDone)
                    await _admins.Mute(user, duration);
                else
                    await _admins.UnMute(user);
            }
        }


        public IServiceProvider BuildServiceProvider() =>
            new ServiceCollection()
                .AddSingleton(_bot)
                .AddSingleton(_commandSvs)
                .AddSingleton<CommandHandler>()
                .AddSingleton<Rainbow>()
                .AddSingleton<BotPrefixes>()
                .AddSingleton<InitializeBot>()
                .AddSingleton(_muteManager)
                .AddSingleton(_admins)
                .BuildServiceProvider();
    }
}