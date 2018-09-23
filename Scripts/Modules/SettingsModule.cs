using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Informatics.Scripts.Utilites;

// ReSharper disable UnusedMember.Global

namespace Informatics.Scripts.Modules {
    
    public class SettingsModule : ModuleBase<SocketCommandContext> {
        private readonly BotPrefixes _prefixes;

        public SettingsModule(BotPrefixes prefixes) {
            _prefixes = prefixes;
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("prefix")]
        [Summary("Меняет префикс на заданный")]
        public async Task CngPrefixAsync(char newPrefix) {
            _prefixes[Context.Guild] = newPrefix;
            await ReplyAsync($"Установлен новый префикс **{newPrefix}**");
        }
    }
}