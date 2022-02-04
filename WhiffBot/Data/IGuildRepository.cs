using Discord.WebSocket;
using WhiffBot.Models;

namespace WhiffBot.Data
{
    public interface IGuildRepository
    {
        public void InitGuild(SocketGuild guild);
        public Guild Get(ulong guildId);
        public void UpdateGuildName(ulong guildId, string guildName);
        public GuildSettings GetSettings(ulong guildId);
        public void SaveRoleAssignmentSettings(ulong guildId, ulong channelId, ulong messageId);
        public void SetAuditLogChannel(ulong guildId, ulong channelId);
        public void AddAssignableRole(ulong guildId, string reaction, ulong roleId);
        public void RemoveAssignableRole(ulong guildId, ulong roleId);
        public void AddAutoResponse(ulong guildId, string message, string response);
        public void RemoveAutoResponse(ulong guildId, string message);
    }
}
