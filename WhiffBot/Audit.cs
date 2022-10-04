﻿using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using WhiffBot.Data;

namespace WhiffBot
{
    public class Audit
    {
        private DiscordSocketClient Client { get; set; }
        private IGuildRepository GuildRepo { get; set; }

        public Audit(DiscordSocketClient client, IGuildRepository guildRepo)
        {
            Client = client;
            GuildRepo = guildRepo;

            Client.GuildMemberUpdated += LogUserNameChange;
            Client.UserJoined += CreateUserLogger("JOIN");
            Client.UserLeft += CreateGuildUserLogger("LEAVE");
            Client.UserBanned += CreateUserGuildLogger("BANNED");
            Client.UserUnbanned += CreateUserGuildLogger("UNBANNED");

            Client.ChannelCreated += CreateChannelLogger("CREATED");
            Client.ChannelDestroyed += CreateChannelLogger("DELETED");
            Client.ChannelUpdated += LogChannelChanged;

            Client.RoleCreated += CreateRoleLogger("CREATED");
            Client.RoleDeleted += CreateRoleLogger("DELETED");
            Client.RoleUpdated += LogRoleChanged;

            Client.MessageReceived += OnMessage;
        }

        /// <summary>
        /// The handler for any audit related commands.
        /// </summary>
        /// <param name="message">The message sent</param>
        /// <returns></returns>
        private async Task OnMessage(SocketMessage message)
        {
            // Only human administrators can manage the role assignment channel.
            if (message.Author.IsBot || !(message.Author as SocketGuildUser).GuildPermissions.Administrator)
                return;

            var channel = message.Channel as SocketTextChannel;
            if (channel == null)
                return;

            var guild = GuildRepo.Get(channel.Guild.Id);

            var parts = message.Content.Split();
            if (parts[0] == $"{guild.Settings.Prefix}setAuditChannel")
            {
                GuildRepo.SetAuditLogChannel(guild.Id, message.Channel.Id);
                await message.AddReactionAsync(new Emoji("\uD83D\uDC4D"));
            }
        }

        /// <summary>
        /// Creates a generic logger for an event that happens on to a user.
        /// </summary>
        /// <param name="property">The action that the user experienced</param>
        /// <returns>A function that will send a message to the audit log regarding the action taken</returns>
        private Func<SocketGuildUser, Task> CreateUserLogger(string property)
        {
            return async user =>
            {
                var guild = GuildRepo.Get(user.Guild.Id);
                if (guild.Settings.AuditChannelId == null)
                    return;

                var channel = user.Guild.GetTextChannel(guild.Settings.AuditChannelId.Value);
                if (channel == null)
                    return;

                await channel.SendMessageAsync($"```{property} [{user.Username}#{user.Discriminator}]```");
            };
        }

        /// <summary>
        /// Creates a generic logger for an event that happens on to a user in a guild.
        /// </summary>
        /// <param name="property">The action that the user experienced</param>
        /// <returns>A function that will send a message to the audit log regarding the action taken</returns>
        private Func<SocketUser, SocketGuild, Task> CreateUserGuildLogger(string property)
        {
            return async (user, discordGuild) =>
            {
                var guild = GuildRepo.Get(discordGuild.Id);
                if (guild.Settings.AuditChannelId == null)
                    return;

                var channel = discordGuild.GetTextChannel(guild.Settings.AuditChannelId.Value);
                if (channel == null)
                    return;

                await channel.SendMessageAsync($"```{property} [{user.Username}#{user.Discriminator}]```");
            };
        }

        private Func<SocketGuild, SocketUser, Task> CreateGuildUserLogger(string property)
        {
            return async (discordGuild, user) => await CreateUserGuildLogger(property)(user, discordGuild);
        }

        /// <summary>
        /// Creates a generic logger for an event that happens on to a channel in a guild.
        /// </summary>
        /// <param name="property">The action that the channel experienced</param>
        /// <returns>A function that will send a message to the audit log regarding the action taken</returns>
        private Func<SocketChannel, Task> CreateChannelLogger(string property)
        {
            return async channel =>
            {
                var editedChannel = channel as SocketGuildChannel;

                var guild = GuildRepo.Get(editedChannel.Guild.Id);
                if (guild.Settings.AuditChannelId == null)
                    return;

                var auditChannel = editedChannel.Guild.GetTextChannel(guild.Settings.AuditChannelId.Value);
                if (auditChannel == null)
                    return;

                
                await auditChannel.SendMessageAsync($"```CHANNEL {property} [{editedChannel.Name}]```");
            };
        }

        /// <summary>
        /// Creates a generic logger for an event that happens on to a role in a guild.
        /// </summary>
        /// <param name="property">The action that the role experienced</param>
        /// <returns>A function that will send a message to the audit log regarding the action taken</returns>
        private Func<SocketRole, Task> CreateRoleLogger(string property)
        {
            return async role =>
            {
                var guild = GuildRepo.Get(role.Guild.Id);
                if (guild.Settings.AuditChannelId == null)
                    return;

                var auditChannel = role.Guild.GetTextChannel(guild.Settings.AuditChannelId.Value);
                if (auditChannel == null)
                    return;

                await auditChannel.SendMessageAsync($"```ROLE {property} [{role.Name}]```");
            };
        }

        /// <summary>
        /// Sends an audit message to the guild's audit channel (if it exists) when a user changes
        /// their nickname.
        /// </summary>
        /// <param name="oldUser">The user before the update</param>
        /// <param name="newUser">The user after the update</param>
        /// <returns>The task completion</returns>
        private async Task LogUserNameChange(Cacheable<SocketGuildUser, ulong> oldUserCached, SocketGuildUser newUser)
        {
            var oldUser = await oldUserCached.GetOrDownloadAsync();
            if (oldUser.Nickname == newUser.Nickname)
                return;

            var guild = GuildRepo.Get(newUser.Guild.Id);
            if (guild.Settings.AuditChannelId == null)
                return;

            var channel = newUser.Guild.GetTextChannel(guild.Settings.AuditChannelId.Value);
            if (channel == null)
                return;

            await channel.SendMessageAsync($"```NAME CHANGE [{oldUser}] [{oldUser.Nickname ?? oldUser.Username}] => [{newUser.Nickname ?? newUser.Username}]```");
        }

        /// <summary>
        /// Sends an audit message to the guild's audit channel (if it exists) when a channel's name changed.
        /// <param name="oldChannel">The channel before the change</param>
        /// <param name="newChannel">The channel after the change</param>
        /// <returns>The task completion</returns>
        private async Task LogChannelChanged(SocketChannel oldChannel, SocketChannel newChannel)
        {
            var oldGuildChannel = oldChannel as SocketGuildChannel;
            var newGuildChannel = newChannel as SocketGuildChannel;

            if ((oldGuildChannel == null || newGuildChannel == null) ||
                (oldGuildChannel.Name == newGuildChannel.Name))
                return;

            var guild = GuildRepo.Get(newGuildChannel.Guild.Id);
            if (guild.Settings.AuditChannelId == null)
                return;

            var channel = newGuildChannel.Guild.GetTextChannel(guild.Settings.AuditChannelId.Value);
            if (channel == null)
                return;

            await channel.SendMessageAsync($"```CHANNEL NAME CHANGE [{oldGuildChannel.Name}] => [{newGuildChannel.Name}]```");
        }

        /// <summary>
        /// Sends an audit message to the guild's audit channel (if it exists) when a role's name changed.
        /// <param name="oldRole">The role before the change</param>
        /// <param name="newRole">The role after the change</param>
        /// <returns>The task completion</returns>
        private async Task LogRoleChanged(SocketRole oldRole, SocketRole newRole)
        {
            if (oldRole.Name == newRole.Name)
                return;

            var guild = GuildRepo.Get(newRole.Guild.Id);
            if (guild.Settings.AuditChannelId == null)
                return;

            var channel = newRole.Guild.GetTextChannel(guild.Settings.AuditChannelId.Value);
            if (channel == null)
                return;

            await channel.SendMessageAsync($"```ROLE NAME CHANGE [{oldRole.Name}] => [{newRole.Name}]```");
        }
    }
}
