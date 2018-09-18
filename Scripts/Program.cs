using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Informatics.Scripts {
    class Program {
        private static bool _isWorking = true;

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args) =>
            new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync() {
            var botConfig = new DiscordSocketConfig {LogLevel = LogSeverity.Info};
            var cmdConfig = new CommandServiceConfig {LogLevel = LogSeverity.Debug};

            var svc = await BuildServices(botConfig, cmdConfig);

            while (_isWorking) {
                await ReadCmdsFromConsole(svc);
            }
        }

        private static async Task<IServiceProvider> BuildServices(DiscordSocketConfig botConfig,
                                                                  CommandServiceConfig cmdConfig) {
            var init = new Initialize(new DiscordSocketClient(botConfig), new CommandService(cmdConfig));
            var svc = init.BuildServiceProvider();

            await svc.GetRequiredService<InitializeBot>().SetupAsync();
            await svc.GetRequiredService<CommandHandler>().SetupAsync();

            init.CheckSummary();
            return svc;
        }

        private static async Task ReadCmdsFromConsole(IServiceProvider svc) {
            var command = await Task.Run(() => Console.ReadLine());
            var bot = svc.GetRequiredService<DiscordSocketClient>();

            switch (command) {
                case "exit":
                case "quit":
                    await Exit(bot);
                    break;
                case "status":
                    Status(bot);
                    break;
                default:
                    if (command.StartsWith("shaperize ")) {
                        await Shaperize(command, bot);
                    }

                    break;
            }
        }

        private static async Task Shaperize(string command, DiscordSocketClient bot) {
            var dirtCmd = command.Substring("shaperize ".Length).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var guildId = ulong.Parse(dirtCmd[0]);
            var nick = dirtCmd[1];
            var guild = bot.GetGuild(guildId);

            Console.WriteLine(new LogMessage(LogSeverity.Warning, "shaperize command",
                                             "Works good only if bot`s role is the first"));

            await guild.Users
                       .First(user => user.Username == nick)
                       .AddRoleAsync(
                           guild.Roles.FirstOrDefault(r => r.Position == guild.Roles.Max(role => role.Position) - 1)
                       );
        }

        private static void Status(DiscordSocketClient bot) {
            Console.WriteLine($"Ping: {bot.Latency} ms");
            Console.WriteLine($"Count of guilds: {bot.Guilds.Count}");
        }

        private static async Task Exit(DiscordSocketClient bot) {
            await bot.StopAsync();
            bot.Dispose();
            _isWorking = false;
            Environment.Exit(0);
        }
    }
}