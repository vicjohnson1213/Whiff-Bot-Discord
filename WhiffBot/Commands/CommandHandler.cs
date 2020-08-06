using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiffBot.Data;

namespace WhiffBot.Commands
{
    public class CommandHandler
    {
        private IGuildRepository GuildRepo { get; set; }
        private DiscordSocketClient Client { get; set; }

        private Dictionary<string, ICommand> Commands = new Dictionary<string, ICommand>();

        public CommandHandler(DiscordSocketClient client, IGuildRepository guildRepo)
        {
            GuildRepo = guildRepo;
            Client = client;

            RegisterCommands();
            Client.MessageReceived += OnMessage;
        }

        /// <summary>
        /// The handler for incoming messages. This will filter out non-commands and
        /// dispatch commands to the appropriate commands.
        /// </summary>
        /// <param name="message">The incoming message</param>
        /// <returns></returns>
        public async Task OnMessage(SocketMessage message)
        {
            var guildChannel = message.Channel as SocketGuildChannel;

            if (guildChannel is null)
                return;

            var guild = GuildRepo.Get(guildChannel.Guild.Id);

            if (guild == null || !message.Content.StartsWith(guild.Settings.Prefix))
                return;

            var parts = message.Content.Split();
            var commandName = parts[0].Substring(1);

            if (!Commands.ContainsKey(commandName))
                return;

            var command = Commands[commandName];
            await command.Run(message, string.Join(' ', parts.Skip(1)));
        }

        private void RegisterCommands()
        {
            var roll = new Roll(GuildRepo);
            Commands.Add(roll.Name, roll);

            var groups = new Groups(GuildRepo);
            Commands.Add(groups.Name, groups);

            var profile = new Profile();
            Commands.Add(profile.Name, profile);

            var straws = new Straws();
            Commands.Add(straws.Name, straws);

            var commands = new Commands(GuildRepo, Commands.Values);
            Commands.Add(commands.Name, commands);
        }
    }
}
