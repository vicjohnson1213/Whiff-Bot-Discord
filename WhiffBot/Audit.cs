using Discord.WebSocket;
using System.Threading.Tasks;
using WhiffBot.Data;

namespace WhiffBot
{
    public class Audit
    {
        private DiscordSocketClient Client { get; set; }
        private IGuildRepository GuildRepo { get; set; }

        public Audit(DiscordSocketClient client, IGuildRepository guildRepo)
        {
            Client = client;
            GuildRepo = guildRepo;

            Client.GuildMemberUpdated += LogMemberNameChange;
        }

        /// <summary>
        /// Sends an audit message to the guild's audit channel (if it exists) when a user changes
        /// their nickname.
        /// </summary>
        /// <param name="oldUser">The user before the update</param>
        /// <param name="newUser">The user after the update</param>
        /// <returns>The task completion</returns>
        private async Task LogMemberNameChange(SocketGuildUser oldUser, SocketGuildUser newUser)
        {
            if (oldUser.Nickname == newUser.Nickname)
                return;

            var guild = GuildRepo.Get(newUser.Guild.Id);
            if (guild.Settings.AuditChannelId == null)
                return;

            var channel = newUser.Guild.GetTextChannel(guild.Settings.AuditChannelId.Value);
            if (channel == null)
                return;

            await channel.SendMessageAsync($"```NAME CHANGE: [{oldUser.Username}] => [{newUser.Nickname ?? newUser.Username}]```");
        }
    }
}
