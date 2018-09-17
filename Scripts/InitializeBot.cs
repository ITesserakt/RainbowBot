using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Informatics.Scripts.Modules;

namespace Informatics.Scripts {
    public class InitializeBot {
        private readonly DiscordSocketClient _bot;
        private readonly Data                _data;
        private readonly UsersRoles          _mutedUsers;
        private readonly AdminsModule _admins;
        
        public InitializeBot(Data data, DiscordSocketClient bot, UsersRoles mutedUsers, AdminsModule admins) {
            _data = data;
            _bot = bot;
            _mutedUsers = mutedUsers;
            _admins = admins;
        }

        public async Task SetupAsync() {
            _bot.Log += LogAsync;
            _bot.Ready += () => {
                Console.WriteLine("[Info] Logged successfuly");
                return Task.CompletedTask;
            };
            
            await _bot.StartAsync();
            await _bot.LoginAsync(TokenType.Bot, _data.Token);
        }

        private Task LogAsync(LogMessage arg) {
            Console.WriteLine($"[{arg.Severity}] {arg.Message}");
            return Task.CompletedTask;
        }
    }
}