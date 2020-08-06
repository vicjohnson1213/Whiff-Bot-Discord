using Discord;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhiffBot
{
    public static class Extensions
    {
        private static Random rng = new Random();

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

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
