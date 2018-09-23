using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using log4net;

namespace Informatics.Scripts.Core {
    public class InitializeBot {
        public static readonly ILog Log = LogManager.GetLogger(typeof(InitializeBot));
        
        private readonly DiscordSocketClient _bot;
        private readonly string _token = "NDgxMTY1MDA0NjEyMTczODI0.DomUPg.05y7qFmEK1GMsH_OTEwPo8L1ZFA";

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
            await _bot.LoginAsync(TokenType.Bot, _token);
        }

        private static Task LogAsync(LogMessage arg) {
            Log.Info(arg.Message);
            return Task.CompletedTask;
        }
    }
}