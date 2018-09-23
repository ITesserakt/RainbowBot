using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Informatics.Scripts.Utilites;

// ReSharper disable UnusedMember.Global

namespace Informatics.Scripts.Modules {
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminsModule : ModuleBase<SocketCommandContext> {
        private readonly MuteManager _muteManager;
        
        public AdminsModule(MuteManager muteManager) {
            _muteManager = muteManager;
        }

        [Command("ban")]
        [Summary("Банит указанного пользователя")]
        public async Task BanAsync(IUser user, int duration = 0, [Remainder] string reason = null) {
            if (user.Id == 316249690092077065) return;
            await Context.Guild.AddBanAsync(user, duration, reason, RequestOptions.Default);
        }

        [Command("kick")]
        [Summary("Кикает указанного пользователя за плохое поведение")]
        [Alias("поджопник")]
        public async Task KickAsync(IUser user, [Remainder] string reason = null) =>
            await Context.Guild.GetUser(user.Id).KickAsync(reason);

        [Command("mute")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Summary("Мутит пользователя на несколько минут\nНа сервере должна быть доступна роль 'Muted'")]
        public async Task Mute(IUser user, float duration) {
            if(!(user is SocketGuildUser guildUser)) return;
            var muteRole = GetMuteRole(guildUser.Guild);

            if (duration > 35791)
                throw new ArgumentOutOfRangeException(nameof(duration),
                                                      "Вы не можете замутить пользователя более чем на 24 дня");

            _muteManager.Save(guildUser, duration);
            await guildUser.RemoveRolesAsync(guildUser.Roles.Where(role => !role.IsEveryone));
            await guildUser.AddRoleAsync(muteRole);

            new Timer {AutoReset = false, Enabled = true, Interval = duration * 60000}.Elapsed += async (_, __) => {
                await UnMute(guildUser);
            }; //костыль ¯\_(ツ)_/¯
        }

        private static SocketRole GetMuteRole(SocketGuild guild) =>
            guild.Roles.First(role => role.Name.ToLower() == "muted");

        [Command("unmute")]
        [Alias("мяу")]
        [Summary("Разрешает пользователю писать в чаты и говорить")]
        public async Task UnMute(IUser user) {
            if (user is SocketGuildUser guildUser) {
                await guildUser.RemoveRoleAsync(GetMuteRole(guildUser.Guild));
                await guildUser.AddRolesAsync(_muteManager.Pop(guildUser).Item1);
            }
        }

        [Command("unban")]
        [Summary("Разбанивает пользователя")]
        public async Task UnBan(IUser user) {
            await Context.Guild.RemoveBanAsync(user);
        }
    }
}