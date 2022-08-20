using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiffBot.Data;

namespace WhiffBot.Commands
{
    public class Groups : ICommand
    {
        private IGuildRepository GuildRepo { get; set; }

        public string Name => "groups";
        public string Description => "Randomly assigns people in a voice channel to teams of a certain size.";

        public Groups(IGuildRepository guildRepo)
        {
            GuildRepo = guildRepo;
        }

        /// <summary>
        /// Randomly assigns people in a voice channel to teams of a certain size.
        /// </summary>
        /// <param name="message">The incoming message</param>
        /// <param name="maxStr">The number of sides on the die</param>
        /// <returns></returns>
        public async Task Run(SocketMessage message, string groupCountStr)
        {
            int groupCount = 2;

            if (!string.IsNullOrWhiteSpace(groupCountStr))
            {
                if (!int.TryParse(groupCountStr, out groupCount))
                {
                    await SendUsage(message.Channel);
                    return;
                }
            }

            var guildUser = message.Author as SocketGuildUser;
            if (guildUser == null)
                return;

            var voiceChannel = guildUser.VoiceChannel;
            if (voiceChannel == null)
            {
                await message.Channel.SendMessageAsync("You must be in a voice channel to use this command.");
                return;
            }

            var activeUsers = voiceChannel.ConnectedUsers
                .Where(u => !u.IsBot)
                .Select(u => u.Username).ToList()
                .Shuffle();

            var groups = new List<List<string>>();
            for (var i = 0; i < activeUsers.Count; i++)
            {
                var groupIdx = i % groupCount;
                var group = groups.ElementAtOrDefault(groupIdx);

                if (group == null)
                {
                    group = new List<string>();
                    groups.Add(group);
                }
                    
                group.Add(activeUsers[i]);
            }

            var res = "";
            foreach (var group in groups)
            {
                res += $"```{string.Join(", ", group)}```";
            }

            await message.Channel.SendMessageAsync(res);
        }

        /// <summary>
        /// Sends the command usage back to the user.
        /// </summary>
        /// <param name="channel">The channel to send to</param>
        /// <returns></returns>
        private async Task SendUsage(ISocketMessageChannel channel)
        {
            var guildChannel = channel as SocketGuildChannel;
            if (guildChannel == null)
                return;

            var guild = GuildRepo.Get(guildChannel.Guild.Id);

            await channel.SendMessageAsync($"Usage:```{guild.Settings.Prefix}{Name} [group count] (defaults to 2)```");

            return;
        }
    }
}
