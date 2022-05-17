using Discord;
using Discord.Rest;
using System;
using WhiffBot.Model.Configuration;

namespace WhiffBot.Discord
{
    public class DiscordServiceProvider
    {
        public async void DoThing()
        {
            var config = new Configuration();
            var client = new DiscordRestClient();
            await client.LoginAsync(TokenType.Bot, config.DiscordClientId);

            var user = client.CurrentUser;
        }
    }
}
