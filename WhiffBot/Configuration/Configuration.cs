using System;

namespace WhiffBot.Configuration
{
    public class Configuration : IConfiguration
    {
        public string DiscordClientId => Environment.GetEnvironmentVariable("WHIFF_BOT_TEST_CLIENT_ID");
        public string ConnectionString => Environment.GetEnvironmentVariable("WHIFF_BOT_TEST_CONNECTION_STRING");
    }
}
