using System;

namespace WhiffBot.Model
{
    public class GuildMember
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public DateTimeOffset? JoinedAt { get; set; }
    }
}
