using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Informatics.Scripts.Core {
    internal class Program {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args) =>
            MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync() {
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()),
                                      new FileInfo($@"{Environment.CurrentDirectory}\log_config.xml"));
            
            var botConfig = new DiscordSocketConfig {LogLevel = LogSeverity.Info};
            var cmdConfig = new CommandServiceConfig {LogLevel = LogSeverity.Debug};

            var svc = await BuildServices(botConfig, cmdConfig);

//            while (_isWorking) {
//                await ReadCmdsFromConsole(svc);
//            }

            await Task.Delay(-1);
        }

        private static async Task<IServiceProvider> BuildServices(DiscordSocketConfig botConfig,
                                                                  CommandServiceConfig cmdConfig) {
            var init = new Initialize(new DiscordSocketClient(botConfig), new CommandService(cmdConfig));
            var svc = init.BuildServiceProvider();

            await svc.GetRequiredService<InitializeBot>().SetupAsync();
            await svc.GetRequiredService<CommandHandler>().SetupAsync();

            init.CheckSummary();
//            await init.CheckMuted();
            return svc;
        }

//        private static async Task ReadCmdsFromConsole(IServiceProvider svc) {
//            var command = await Task.Run(() => Console.ReadLine());
//            var bot = svc.GetRequiredService<DiscordSocketClient>();
//            var comSvc = svc.GetRequiredService<CommandService>();
//
//            switch (command) {
//                case "exit":
//                case "quit":
//                    await Exit(bot);
//                    break;
//                default:
//                    if (command.StartsWith("shaperize ")) {
//                        await Shaperize(command, bot);
//                    }
//                    
//                    break;
//            }
//        }

        private static async Task Shaperize(string command, DiscordSocketClient bot) {
            var dirtCmd = command.Substring("shaperize ".Length).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var guildId = ulong.Parse(dirtCmd[0]);
            var nick = ulong.Parse(dirtCmd[1]);
            var guild = bot.GetGuild(guildId);
            var botMaxRoleNum = guild.Users
                                  .FirstOrDefault(user => user.Username == bot.CurrentUser.Username)
                                  ?.Roles.Max(role => role.Position);//yeah, max = min

            Log.Info("Works good only if bot`s role is the first");
            foreach (var role in guild.Roles) {
                Log.Debug($"{role.Name} {role.Position}");
            }

            await guild.GetUser(nick)
                       .AddRoleAsync(
                           guild.Roles.First(role => !role.IsEveryone && role.Position == botMaxRoleNum - 1)
                       );
        }

        private static void Status(DiscordSocketClient bot) {
            Log.Info($"Ping: {bot.Latency} ms");
            Log.Info($"Count of guilds: {bot.Guilds.Count}");
        }

        private static async Task Exit(DiscordSocketClient bot) {
            await bot.StopAsync();
            bot.Dispose();
            Environment.Exit(0);
        }
    }
}