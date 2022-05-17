using Discord;
using Discord.Rest;
using WhiffBot.Model;

namespace WhiffBot.API
{
    public static class DiscordMappers
    {
        public static GuildMember ToGuildMember(this IGuildUser guildUser)
        {
            return new GuildMember
            {
                Id = guildUser.Id,
                Username = guildUser.Username,
                JoinedAt = guildUser.JoinedAt
            };
        }
    }
}
