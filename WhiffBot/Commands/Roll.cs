using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using WhiffBot.Data;

namespace WhiffBot.Commands
{
    class Roll : ICommand
    {
        private IGuildRepository GuildRepo { get; set; }

        public string Name => "roll";
        public string Description => "Rolls an n sided die.";

        public Roll(IGuildRepository guildRepo)
        {
            GuildRepo = guildRepo;
        }

        /// <summary>
        /// Generates a random number between 1 and the specified max. Sending the result back
        /// to the user.
        /// </summary>
        /// <param name="message">The incoming message</param>
        /// <param name="maxStr">The number of sides on the die</param>
        /// <returns></returns>
        public async Task Run(SocketMessage message, string maxStr)
        {
            var max = 20;

            if (!string.IsNullOrWhiteSpace(maxStr))
            {
                if (!int.TryParse(maxStr, out max))
                {
                    await SendUsage(message.Channel);
                    return;
                }
            }

            var rand = new Random();
            var res = rand.Next(1, max);

            await message.Channel.SendMessageAsync($"```{res}```");
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

            await channel.SendMessageAsync($"Usage:```{guild.Settings.Prefix}{Name} [number of sides] (defaults to 20)```");

            return;
        }
    }
}
