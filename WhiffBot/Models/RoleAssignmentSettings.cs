using System;
using System.Collections.Generic;
using System.Text;

namespace WhiffBot.Models
{
    public class RoleAssignmentSettings
    {
        public ulong? ChannelId { get; set; }
        public ulong? MessageId { get; set; }
        public List<RoleAssignmentRole> Roles { get; set; }
    }

    public class RoleAssignmentRole
    {
        public string Reaction { get; set; }
        public ulong RoleId { get; set; }
    }
}
