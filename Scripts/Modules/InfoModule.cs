using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// ReSharper disable UnusedMember.Global

namespace Informatics.Scripts.Modules {
    public class InfoModule : ModuleBase<SocketCommandContext> {
        private readonly CommandService Cmds;
        private readonly BotPrefixes _prefixes;

        public InfoModule(CommandService cmds, BotPrefixes prefixes) {
            Cmds = cmds;
            _prefixes = prefixes;
        }

        [Command("help")]
        [Summary("Выводит список всех команд, если имя команды не передано, иначе - описание команды")]
        [Alias("neko")]
        public async Task HelpAsync([Remainder] string strCmd = null) {
            if (await OnNullHelp(strCmd)) return;

            Cmds.Commands
                .Where(
                    c => c.Name == strCmd || c.Aliases.FirstOrDefault(cmd => cmd == strCmd) != null)
                .ToList().ForEach(async cmd => await OnHelp(cmd));
        }

        private async Task OnHelp(CommandInfo command) {
            var guild = Context.Guild;

            if (command != null)
                await ReplyAsync(
                    $"**{CommandToString(command)}**\n{command.Summary}", true);
            else
                await ReplyAsync(
                    $"Данной команды не существует; введите **{_prefixes[guild]}help** для просмотра списка команд");
        }

        private async Task<bool> OnNullHelp(string strCmd) {
            if (!string.IsNullOrWhiteSpace(strCmd)) return false;

            var cmdList = Cmds.Commands.Select(CommandToString);
            await ReplyAsync($"Список всех команд: \n{string.Join(",\n", cmdList)}");
            return true;
        }

        private static string CommandToString(CommandInfo cmdInfo) {
            var @params = cmdInfo?.Parameters;
            var result = new StringBuilder($"{cmdInfo?.Name} ", 32);

            result.Append("(");
            result.AppendJoin(", ", @params?.Select(p => p.IsOptional
                                                       ? $"<{p.Name} : {p.Type.Name}>"
                                                       : $"{p.Name} : {p.Type.Name}"));
            result.Append(")");

            return result.ToString();
        }

        [Command("role id")]
        [RequireOwner]
        [Summary("Используется для разработки; возвращает id указанной роли")]
        public async Task GetRoleId(IRole role) {
            await ReplyAsync($"{((SocketRole) role).Id}");
        }
    }
}