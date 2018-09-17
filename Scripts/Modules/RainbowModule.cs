using System;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

// ReSharper disable UnusedMember.Global

namespace Informatics.Scripts.Modules {
    public class RainbowModule : ModuleBase<SocketCommandContext> {
        private readonly Rainbow _rainbow;
        public RainbowModule(Rainbow rainbow) {
            _rainbow = rainbow;
        }

        [Command("rainbow setup")]
        [Summary("Устанавливает, если переданы, задержку и скорость изменения цвета роли")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetupAsync(IRole role, int delay = 0, float step = 0) {
            if (delay < 60 && delay != 0)
                throw new ArgumentOutOfRangeException(nameof(delay), "Задержка не должна быть меньше 60мс");

            var rainbowStep = Math.Clamp(step, 0.001f, 0.9f);
            _rainbow.Stop();
            
            if (Math.Abs(step) > 0.001f)
                _rainbow.Step = rainbowStep;
            
            if (delay != 0)
                _rainbow.Delay = delay;
            _rainbow.Start(role);

            await ReplyAsync($"Задержка: {_rainbow.Delay}мс; Скорость именения: {_rainbow.Step}");
        }

        [Command("rainbow stop")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task Stop() {
            _rainbow.Stop();
            return Task.CompletedTask;
        }
    }
}
