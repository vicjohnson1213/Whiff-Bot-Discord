using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiffBot.Data;
using WhiffBot.Models;

namespace WhiffBot
{
    public class AutoResponse
    {
        private readonly DiscordSocketClient _client;
        private readonly IGuildRepository _guildRepo;

        public AutoResponse(DiscordSocketClient client, IGuildRepository guildRepo)
        {
            _client = client;
            _guildRepo = guildRepo;

            _client.MessageReceived += OnMessage;
        }

        public async Task OnMessage(SocketMessage message)
        {
            if (message.Author.IsBot)
                return;

            var guildChannel = message.Channel as SocketGuildChannel;

            if (guildChannel is null)
                return;

            var guild = _guildRepo.Get(guildChannel.Guild.Id);

            if (guild == null)
                return;


            if (message.Content.StartsWith(guild.Settings.Prefix))
            {
                await HandleCommand(guild, message);
                return;
            }

            if (guild.AutoResponses.ContainsKey(message.Content))
                await message.Channel.SendMessageAsync(guild.AutoResponses[message.Content]);
        }

        private async Task HandleCommand(Guild guild, SocketMessage message)
        {
            if (message.Content.StartsWith($"{guild.Settings.Prefix}responses"))
                await ListResponses(guild, message);
            else if (message.Content.StartsWith($"{guild.Settings.Prefix}randomResponse"))
                await SendRandomResponse(guild, message);

            var isAdmin = (message.Author as SocketGuildUser).GuildPermissions.Administrator;

            if (!isAdmin)
                return;

            if (message.Content.StartsWith($"{guild.Settings.Prefix}addResponse"))
                await AddResponse(guild, message);
            else if (message.Content.StartsWith($"{guild.Settings.Prefix}removeResponse"))
                await RemoveResponse(guild, message);
        }

        private async Task AddResponse(Guild guild, SocketMessage message)
        {
            var parts = message.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3)
            {
                await message.Channel.SendMessageAsync($"```Usage: {guild.Settings.Prefix}addResponse <message> <response>```");
                return;
            }

            _guildRepo.AddAutoResponse(guild.Id, parts[1], string.Join(' ', parts.Skip(2)));

            await message.AddReactionAsync(new Emoji("👍"));
        }

        private async Task RemoveResponse(Guild guild, SocketMessage message)
        {
            var parts = message.Content.Split();
            if (parts.Length < 2)
            {
                await message.Channel.SendMessageAsync($"```Usage: {guild.Settings.Prefix}removeResponse <message>```");
                return;
            }

            _guildRepo.RemoveAutoResponse(guild.Id, parts[1]);

            await message.AddReactionAsync(new Emoji("👍"));
        }

        private async Task ListResponses(Guild guild, SocketMessage message)
        {
            var responses = _guildRepo.GetAutoResponses(guild.Id);

            var response = "```";

            var messages = new List<string>(responses.Keys);
            messages.Sort();

            foreach (var r in responses.Keys.ToList().OrderBy(k => k))
                response += $"{r}\n";

            response += "```";

            await message.Channel.SendMessageAsync(response);
        }

        private async Task SendRandomResponse(Guild guild, SocketMessage message)
        {
            var responses = _guildRepo.GetAutoResponses(guild.Id);

            var rand = new Random();
            var response = responses.ElementAt(rand.Next(0, responses.Count)).Value;

            await message.Channel.SendMessageAsync(response);
        }
    }
}