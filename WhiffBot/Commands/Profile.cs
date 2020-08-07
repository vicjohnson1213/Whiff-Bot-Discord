using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace WhiffBot.Commands
{
    public class Profile : ICommand
    {
        public string Name => "profile";
        public string Description => "Displays information about a user.";

        /// <summary>
        /// Sends profile information about a user.
        /// </summary>
        /// <param name="message">The incoming message</param>
        /// <param name="args">Any arguments for the command</param>
        /// <returns></returns>
        public async Task Run(SocketMessage message, string args)
        {
            var mentionedUser = message.MentionedUsers.FirstOrDefault();
            var profileUser = mentionedUser ?? message.Author;
            var guildUser = profileUser as SocketGuildUser;

            if (guildUser == null)
                return;

            var joinTime = guildUser.JoinedAt;
            var registerTime = guildUser.CreatedAt;

            var format = "MMMM dd, yyyy";

            var roles = guildUser.Roles
                .OrderBy(r => r.Position)
                .Select(r => r.Mention)
                .Reverse()
                .ToList();

            var eb = new EmbedBuilder();
            eb.WithAuthor(guildUser);
            eb.WithDescription(guildUser.Mention);
            eb.WithColor(0x00AE86);
            eb.AddField("Joined", joinTime.Value.ToString(format), true);
            eb.AddField("Registered", registerTime.ToString(format), true);
            eb.WithFooter("Don't shoot the messenger");
            eb.WithCurrentTimestamp();
            eb.AddField($"Roles [{roles.Count}]", string.Join(' ', roles));

            await message.Channel.SendMessageAsync("", false, eb.Build());
        }
    }
}
