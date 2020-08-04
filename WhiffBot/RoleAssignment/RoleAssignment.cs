using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using WhiffBot.Data;
using WhiffBot.Models;

namespace WhiffBot.RoleAssignment
{
    /// <summary>
    /// A bot module responsible for the management of a message that users can react to in order to assign themselves
    /// roles. Because Discord doesn't have very specific permissions around assigning roles, we must work around them.
    /// </summary>
    public class RoleAssignment
    {
        private IGuildRepository GuildRepo { get; set; }
        private DiscordSocketClient Client { get; set; }

        public RoleAssignment(DiscordSocketClient client, IGuildRepository guildRepo)
        {
            GuildRepo = guildRepo;
            Client = client;

            Client.MessageReceived += OnMessage;
            Client.ReactionAdded += OnReactionAdded;
            Client.ReactionRemoved += OnReactionRemoved;
        }

        /// <summary>
        /// The handling method for when a user adds a reaction to the role assignment message. The bot will
        /// assign the associated role to the user if it's assignable.
        /// </summary>
        /// <param name="message">The message the user reacted to</param>
        /// <param name="channel">The channel the message/reaction are in</param>
        /// <param name="reaction">The reaction by the user</param>
        /// <returns></returns>
        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var guildChannel = channel as SocketTextChannel;
            var guild = GuildRepo.Get(guildChannel.Guild.Id);
            var user = reaction.User.Value as SocketGuildUser;

            if (message.Id != guild.Settings.RoleAssignment.MessageId)
                return;

            var roleToAdd = guild.Settings.RoleAssignment.Roles.Find(r => r.Reaction == reaction.Emote.Name);

            // If a user adds an unknown reaction, just delete it.
            if (roleToAdd == null)
            {
                var guildMessage = await channel.GetMessageAsync(message.Id);
                await guildMessage.RemoveReactionAsync(reaction.Emote, user);
                return;
            }

            var discordRoleToAdd = guildChannel.Guild.GetRole(roleToAdd.RoleId);
            await user.AddRoleAsync(discordRoleToAdd);
        }

        /// <summary>
        /// The handling method for when a user removes their reaction to the role assignment message. The bot will
        /// remove the associated role from the user if it's assignable.
        /// </summary>
        /// <param name="message">The message the user removed their reaction from</param>
        /// <param name="channel">The channel the message/reaction are in</param>
        /// <param name="reaction">The reaction removed by the user</param>
        /// <returns></returns>
        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var guildChannel = channel as SocketTextChannel;

            var guild = GuildRepo.Get(guildChannel.Guild.Id);

            if (message.Id != guild.Settings.RoleAssignment.MessageId)
                return;

            var roleToRemove = guild.Settings.RoleAssignment.Roles.Find(r => r.Reaction == reaction.Emote.Name);

            if (roleToRemove == null)
                return;

            var discordRoleToRemove = guildChannel.Guild.GetRole(roleToRemove.RoleId);
            var user = reaction.User.Value as SocketGuildUser;
            await user.RemoveRoleAsync(discordRoleToRemove);
        }

        /// <summary>
        /// The handling method for when a managing messages in the assignment channel. Guild administrators
        /// can initialize the assignment, add assignable roles, or remove assignable roles.
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
            if (parts[0] == $"{guild.Settings.Prefix}initRoleAssignment")
            {
                await InitForGuild(channel, guild);
                _ = channel.DeleteMessageAsync(message);
                return;
            }

            // Ignore any requests to add/remove roles if they aren't in the designated channel.
            if (guild.Settings.RoleAssignment.ChannelId != channel.Id)
                return;

            if (parts[0] == $"{guild.Settings.Prefix}add")
            {
                await AddNewRole(channel.Guild, guild, parts[1], message.MentionedRoles.FirstOrDefault());
            }
            else if (parts[0] == $"{guild.Settings.Prefix}remove")
            {
                await RemoveRole(channel.Guild, guild, message.MentionedRoles.FirstOrDefault());
            }

            _ = channel.DeleteMessageAsync(message);

        }

        /// <summary>
        /// Adds a new role to the database and updates the assignment message to accept the new role.
        /// </summary>
        /// <param name="discordGuild">The guild being operated on</param>
        /// <param name="guild">The stored guild information</param>
        /// <param name="reaction">The reaction emoji</param>
        /// <param name="role">The role to make assignable</param>
        /// <param name="messageToDelete">The sent message that should be deleted</param>
        /// <returns></returns>
        private async Task AddNewRole(SocketGuild discordGuild, Guild guild, string reaction, SocketRole role)
        {
            if (
                string.IsNullOrWhiteSpace(reaction) ||
                reaction.Length > 2 ||
                role == null ||
                guild.Settings.RoleAssignment.Roles.Find(r => r.RoleId == role.Id) != null)
            {
                return;
            }

            GuildRepo.AddAssignableRole(discordGuild.Id, reaction, role.Id);
            var message = await UpdateAssignerMessage(discordGuild);
            _ = message.AddReactionAsync(new Emoji(reaction));
        }

        /// <summary>
        /// Removes a role from those that are assignable and updates the assignment mesasge accordingly.
        /// </summary>
        /// <param name="discordGuild">The guild being operated on</param>
        /// <param name="guild">The stored guild information</param>
        /// <param name="role">The role to remove</param>
        /// <returns></returns>
        private async Task RemoveRole(SocketGuild discordGuild, Guild guild, SocketRole role)
        {
            if (role == null)
                return;

            GuildRepo.RemoveAssignableRole(discordGuild.Id, role.Id);
            var message = await UpdateAssignerMessage(discordGuild);
            var emojiToRemove = guild.Settings.RoleAssignment.Roles.Find(r => r.RoleId == role.Id);
            _ = message.RemoveAllReactionsForEmoteAsync(new Emoji(emojiToRemove.Reaction));
        }

        /// <summary>
        /// Creates a new assignment message for a guild.
        /// </summary>
        /// <param name="channel">The channel to create the assignment message in</param>
        /// <param name="guild">The stored guild information</param>
        /// <returns></returns>
        private async Task InitForGuild(SocketTextChannel channel, Guild guild)
        {
            var content = CreateAssignerMessage(channel.Guild, guild.Settings.RoleAssignment);
            var result = await channel.SendMessageAsync(content);
            await result.AddReactionsAsync(guild.Settings.RoleAssignment.Roles.Select(r => new Emoji(r.Reaction)));

            GuildRepo.SaveRoleAssignmentSettings(guild.Id, channel.Id, result.Id);
        }

        /// <summary>
        /// Updates the assignment message with any potential changes to the assignable roles.
        /// </summary>
        /// <param name="discordGuild">The guild that is being operated on</param>
        /// <returns>The assignment message</returns>
        private async Task<IMessage> UpdateAssignerMessage(SocketGuild discordGuild)
        {
            var guild = GuildRepo.Get(discordGuild.Id);
            var assignerChannel = discordGuild.GetChannel(guild.Settings.RoleAssignment.ChannelId.Value) as SocketTextChannel;

            IUserMessage assignerMessage = assignerChannel.GetCachedMessage(guild.Settings.RoleAssignment.MessageId.Value) as SocketUserMessage;

            if (assignerMessage == null)
                assignerMessage = await assignerChannel.GetMessageAsync(guild.Settings.RoleAssignment.MessageId.Value) as RestUserMessage;

            var newContent = CreateAssignerMessage(discordGuild, guild.Settings.RoleAssignment);
            await assignerMessage.ModifyAsync(m => m.Content = newContent);

            return assignerMessage;
        }

        /// <summary>
        /// Generates the message that will be kept in the assigner message.
        /// </summary>
        /// <param name="discordGuild">The guild being operated on</param>
        /// <param name="settings">The role assignment settings for the guild</param>
        /// <returns></returns>
        private string CreateAssignerMessage(SocketGuild discordGuild, RoleAssignmentSettings settings)
        {
            var message = "**Here are the roles you can assign to yourself:**";
            message += "\n\n";

            foreach (var role in settings.Roles)
            {
                var discordRole = discordGuild.GetRole(role.RoleId);
                message += $"{role.Reaction} -> {discordRole.Mention}\n";
            }

            message += "\nRemoving your reaction to for any of these roles will also remove that role.";

            return message;
        }
    }
}
