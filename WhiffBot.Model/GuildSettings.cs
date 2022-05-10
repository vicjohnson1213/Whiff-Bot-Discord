namespace WhiffBot.Model
{
    public class GuildSettings
    {
        public char Prefix { get; set; }
        public ulong? AuditChannelId { get; set; }
        public RoleAssignmentSettings RoleAssignment { get; set; }
    }
}
