using Discord;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Informatics.Scripts {
    public class Data {
        private readonly DiscordSocketClient _bot;

        public Data(DiscordSocketClient bot) {
            _bot = bot;
        }

        public char DefaultPrefix => '/';

        public string Token => "NDgxMTY1MDA0NjEyMTczODI0.Dlyk8Q.rohPktpHd9zkHbW0ARGm_DVcFYM";
    }
}