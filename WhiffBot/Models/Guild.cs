using System;
using System.Collections.Generic;
using System.Text;

namespace WhiffBot.Models
{
    public class Guild
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public GuildSettings Settings { get; set; }
    }
}
