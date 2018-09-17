using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Informatics.Scripts {
    public class InitializeBot {
        private readonly DiscordSocketClient _bot;
        private const string Token = "NDgxMTY1MDA0NjEyMTczODI0.DoFv-Q.WiSLhJTg8X2K2UIUqqjQAKfIP8U";

        public InitializeBot(DiscordSocketClient bot) {
            _bot = bot;
        }

        public async Task SetupAsync() {
            _bot.Log += LogAsync;
            _bot.Ready += async () => {
                Console.WriteLine("[Info] Logged successfully");
                await _bot.SetGameAsync("/help", type: ActivityType.Watching);
            };

            await _bot.StartAsync();
            await _bot.LoginAsync(TokenType.Bot, Token);
        }

        private static Task LogAsync(LogMessage arg) {
            Console.WriteLine($"[{arg.Severity}] {arg.Message}");
            return Task.CompletedTask;
        }
    }
}