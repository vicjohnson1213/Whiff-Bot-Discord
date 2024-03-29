﻿using Discord;
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
        /// <returns>A Task when all reactions have been added.</returns>
        public static async Task AddReactionsAsync(this IMessage message, IEnumerable<IEmote> reactions)
        {
            foreach (var reaction in reactions)
                await message.AddReactionAsync(reaction);
        }

        /// <summary>
        /// Attempts to parse an emote from a string. If that fails, the input is assumed
        /// to be an emoji.
        /// </summary>
        /// <param name="str">The string to convert to an IEmote</param>
        /// <returns></returns>
        public static IEmote ToEmojiOrEmote(this string str)
        {
            Emote emote;

            if (Emote.TryParse(str, out emote))
                return emote;

            return new Emoji(str);
        }

        /// <summary>
        /// Shuffles a list of elements.
        /// </summary>
        /// <typeparam name="T">The type of the element in the list.</typeparam>
        /// <param name="list">The list to shuffle</param>
        /// <returns>The shuffled list</returns>
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
