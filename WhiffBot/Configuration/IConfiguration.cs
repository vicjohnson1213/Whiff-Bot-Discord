using System;
using System.Collections.Generic;
using System.Text;

namespace WhiffBot.Configuration
{
    public interface IConfiguration
    {
        public string DiscordClientId { get; }
        public string ConnectionString { get; }
    }
}
