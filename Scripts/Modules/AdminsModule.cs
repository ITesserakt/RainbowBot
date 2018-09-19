using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

// ReSharper disable UnusedMember.Global

namespace Informatics.Scripts.Modules {
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminsModule : ModuleBase<SocketCommandContext> {
        private readonly UsersRoles _usersRoles;
        
        public AdminsModule(UsersRoles usersRoles) {
            _usersRoles = usersRoles;
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
        public async Task Mute(IUser user, double duration) {
            var muteRole = Context.Guild.Roles.First(role => role.Name == "Muted");
            if(!(user is SocketGuildUser guildUser)) return;

            if (duration > 35791)
                throw new ArgumentOutOfRangeException(nameof(duration),
                                                      "Вы не можете замутить пользователя более чем на 24 дня");

            _usersRoles.Add(guildUser, duration);

            await guildUser.RemoveRolesAsync(guildUser.Roles.Where(r => !r.IsEveryone));
            await guildUser.AddRoleAsync(muteRole);

            await Task.Delay((int) (duration * 60000));

            await guildUser.RemoveRoleAsync(muteRole);
            await guildUser.AddRolesAsync(_usersRoles.GetRoles(guildUser));
        }

        [Command("unmute")]
        [Alias("мяу")]
        [Summary("Разрешает пользователю писать в чаты и говорить")]
        public async Task UnMute(IUser user) {
            var muteRole = Context.Guild.Roles.First(role => role.Name == "Muted");
            if (user is SocketGuildUser guildUser) {
                await guildUser.RemoveRoleAsync(muteRole);
                await guildUser.AddRolesAsync(_usersRoles.Pop(guildUser));
            }
        }

        [Command("unban")]
        [Summary("Разбаниват пользователя")]
        public async Task UnBan(IUser user) {
            await Context.Guild.RemoveBanAsync(user);
        }
    }

    public class UsersRoles  {
        private readonly Dictionary<SocketGuildUser, (IEnumerable<SocketRole>, double)> _roles =
            new Dictionary<SocketGuildUser, (IEnumerable<SocketRole>, double)>();

        public void Add(SocketGuildUser user, double duration) {
            _roles.Add(user, (user.Roles.Where(r => !r.IsEveryone), duration));
        }

        public IEnumerable<SocketRole> Pop(SocketGuildUser user) {
            _roles.Remove(user, out var roles);
            return roles.Item1;
        }

        public bool Contains(SocketGuildUser user) {
            return _roles.ContainsKey(user);
        }

        public IEnumerable<SocketRole> GetRoles(SocketGuildUser user) => _roles[user].Item1;

        public double GetMuteDuration(SocketGuildUser user) => _roles[user].Item2;
    }
}