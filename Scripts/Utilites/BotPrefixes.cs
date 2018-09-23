using System.Collections.Generic;
using Discord.WebSocket;

namespace Informatics.Scripts.Utilites {
    public class BotPrefixes{

        private Dictionary<SocketGuild, char> prefixes = new Dictionary<SocketGuild, char>();

        public char Default => '/';

        public char this[SocketGuild guild]
        {
            get => prefixes.ContainsKey(guild) ? prefixes[guild] : Default;
            set => prefixes[guild] = value;
        }
    }   
}