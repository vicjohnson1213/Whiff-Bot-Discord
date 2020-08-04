using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhiffBot
{
    public static class Extensions
    {
        /// <summary>
        /// Adds multiple reactions to a message.
        /// </summary>
        /// <param name="message">The message to add reactions to</param>
        /// <param name="reactions">The reactions to add</param>
        /// <returns></returns>
        public static async Task AddReactionsAsync(this IMessage message, IEnumerable<IEmote> reactions)
        {
            foreach (var reaction in reactions)
                await message.AddReactionAsync(reaction);
        }
    }
}
