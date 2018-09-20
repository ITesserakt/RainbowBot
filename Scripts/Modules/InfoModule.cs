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
        private readonly CommandService _cmds;
        private readonly BotPrefixes _prefixes;

        public InfoModule(CommandService cmds, BotPrefixes prefixes) {
            _cmds = cmds;
            _prefixes = prefixes;
        }

        [Command("help")]
        [Summary("Выводит список всех команд, если имя команды не передано, иначе - описание команды")]
        [Alias("neko")]
        public async Task HelpAsync([Remainder] string strCmd = null) {
            if (await OnNullHelp(strCmd)) return;

            await OnHelp(_cmds.Commands.FirstOrDefault(c => c.Name == strCmd || c.Aliases.Contains(strCmd)));
        }

        private async Task OnHelp(CommandInfo command) {
            var guild = Context.Guild;

            if (command != null)
                await ReplyAsync($"**{CommandToString(command)}**\n{command.Summary}");
            else
                await ReplyAsync(
                    $"Данной команды не существует; введите **{_prefixes[guild]}help** для просмотра списка команд");
        }

        private async Task<bool> OnNullHelp(string strCmd) {
            if (!string.IsNullOrWhiteSpace(strCmd)) return false;

            var cmdList = _cmds.Commands.Select(CommandToString);
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
    }
}