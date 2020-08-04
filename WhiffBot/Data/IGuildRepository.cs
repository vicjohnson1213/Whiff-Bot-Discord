using WhiffBot.Models;

namespace WhiffBot.Data
{
    public interface IGuildRepository
    {
        public Guild Get(ulong guildId);
        public GuildSettings GetSettings(ulong guildId);
        public void SaveRoleAssignmentSettings(ulong guildId, ulong channelId, ulong messageId);
        public void AddAssignableRole(ulong guildId, string reaction, ulong roleId);
        public void RemoveAssignableRole(ulong guildId, ulong roleId);
    }
}
