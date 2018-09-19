using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using log4net;
using log4net.Core;

namespace Informatics.Scripts {
    public class InitializeBot {
        public static readonly ILog Log = LogManager.GetLogger(typeof(InitializeBot));
        
        private readonly DiscordSocketClient _bot;
        private const string Token = "NDkxOTM1NzkwMDg4MzIzMDc0.DoQQZw.sR9xOJguj-KKMoEI6Zbl-amySoM";

        public InitializeBot(DiscordSocketClient bot) {
            _bot = bot;
        }

        public async Task SetupAsync() {
            _bot.Log += LogAsync;
            _bot.Ready += async () => {
                Log.Info("Logged successfully");
                await _bot.SetGameAsync("/help", type: ActivityType.Watching);
            };

            await _bot.StartAsync();
            await _bot.LoginAsync(TokenType.Bot, Token);
        }

        private static Task LogAsync(LogMessage arg) {
            Log.Info(arg.Message);
            return Task.CompletedTask;
        }
    }
}