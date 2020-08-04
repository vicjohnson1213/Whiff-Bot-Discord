using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using WhiffBot.Commands;
using Conf = WhiffBot.Configuration;
using WhiffBot.Data;
using RA = WhiffBot.RoleAssignment;

namespace WhiffBot
{
    class WhiffBot
    {
        private DiscordSocketClient Client { get; set; }

        static void Main(string[] args) => new WhiffBot().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var config = new Conf.Configuration();

            var discordConfig = new DiscordSocketConfig { MessageCacheSize = 10 };
            Client = new DiscordSocketClient(discordConfig);

            Client.Log += Log;

            await Client.LoginAsync(TokenType.Bot, config.DiscordClientId);
            await Client.StartAsync();

            InitModules(config);

            await Task.Delay(-1);
        }


        private void InitModules(Conf.Configuration config)
        {
            var guildRepo = new GuildRepository(config);
            new Audit(Client, guildRepo);
            new RA.RoleAssignment(Client, guildRepo);
            new CommandHandler(Client, guildRepo);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
