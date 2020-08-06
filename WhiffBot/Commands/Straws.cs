using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiffBot.Data;

namespace WhiffBot.Commands
{
    public class Straws : ICommand
    {
        public string Name => "straws";
        public string Description => "Draws straws for all users in a voice channel.";

        /// <summary>
        /// Draws straws for all users in a voice channel.
        /// </summary>
        /// <param name="message">The incoming message</param>
        /// <param name="maxStr">The number of sides on the die</param>
        /// <returns></returns>
        public async Task Run(SocketMessage message, string args)
        {
            var guildUser = message.Author as SocketGuildUser;
            if (guildUser == null)
                return;

            var voiceChannel = guildUser.VoiceChannel;
            if (voiceChannel == null)
            {
                await message.Channel.SendMessageAsync("You must be in a voice channel to use this command.");
                return;
            }

            var activeUsers = voiceChannel.Users
                .Where(u => !u.IsBot)
                .Select(u => u.Username)
                .ToList()
                .Shuffle();

            var padSize = activeUsers
                .Select(u => u.Length)
                .Max() + 2;

            var res = "```";

            for (var i = 0; i < activeUsers.Count; i++)
            {
                var strawSize = activeUsers.Count - i + 3;
                var user = activeUsers[i];
                res += $"{user.PadRight(padSize)}{"".PadRight(strawSize, '=')}\n";
            }

            res += "```";

            await message.Channel.SendMessageAsync(res);
        }
    }
}
