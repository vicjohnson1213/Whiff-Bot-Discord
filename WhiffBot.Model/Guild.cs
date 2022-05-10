using System;
using System.Collections.Generic;
using System.Text;

namespace WhiffBot.Model
{
    public class Guild
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public GuildSettings Settings { get; set; }
        public Dictionary<string, string> AutoResponses { get; set; }
    }
}
