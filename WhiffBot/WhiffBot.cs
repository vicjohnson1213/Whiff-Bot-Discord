﻿using Discord;
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
        private IGuildRepository GuildRepo { get; set; }
        private DiscordSocketClient Client { get; set; }

        static void Main(string[] args) => new WhiffBot().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var config = new Conf.Configuration();

            var discordConfig = new DiscordSocketConfig { MessageCacheSize = 10 };
            Client = new DiscordSocketClient(discordConfig);
            GuildRepo = new GuildRepository(config);

            Client.Log += Log;

            await Client.LoginAsync(TokenType.Bot, config.DiscordClientId);
            await Client.StartAsync();
            Client.Ready += Init();
            Client.Disconnected += HandleDisconnect;
            Client.JoinedGuild += InitGuild;

            await Task.Delay(-1);
        }

        private async Task HandleDisconnect(Exception e)
        {
            await Log(new LogMessage(LogSeverity.Critical, "WhiffBot", "Restarting", e));
            Environment.Exit(1);
        }

        private Func<Task> Init()
        {
            return () =>
            {
                InitGuilds();
                InitModules();
                return Task.CompletedTask;
            };
        }

        private Task InitGuilds()
        {
            foreach (var discordGuild in Client.Guilds)
                InitGuild(discordGuild);
            return Task.CompletedTask;
        }

        private Task InitGuild(SocketGuild discordGuild)
        {
            var guild = GuildRepo.Get(discordGuild.Id);

            if (guild == null)
                GuildRepo.InitGuild(discordGuild);

            return Task.CompletedTask;
        }

        private void InitModules()
        {
            new Audit(Client, GuildRepo);
            new RA.RoleAssignment(Client, GuildRepo);
            new CommandHandler(Client, GuildRepo);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
