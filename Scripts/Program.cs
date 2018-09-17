using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Informatics.Scripts {
    class Program {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync() {
            var botConfig = new DiscordSocketConfig {LogLevel = LogSeverity.Info};
            var cmdConfig = new CommandServiceConfig {LogLevel = LogSeverity.Debug};

            var init = new Initialize(new DiscordSocketClient(botConfig), new CommandService(cmdConfig))
                .BuildServiceProvider();

            await init.GetRequiredService<InitializeBot>().SetupAsync();
            await init.GetRequiredService<CommandHandler>().SetupAsync();

            await Task.Delay(-1);
        }
    }
}