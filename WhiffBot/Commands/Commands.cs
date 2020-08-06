using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiffBot.Data;

namespace WhiffBot.Commands
{
    public class Commands : ICommand
    {
        private IGuildRepository GuildRepo { get; set; }
        private IEnumerable<ICommand> _commands;

        public string Name => "commands";
        public string Description => "Sends all available commands to the user.";

        public Commands(IGuildRepository guildRepo, IEnumerable<ICommand> commands)
        {
            _commands = commands;
            GuildRepo = guildRepo;
        }

        public async Task Run(SocketMessage message, string args)
        {
            var prefix = "";
            var guildChannel = message.Channel as SocketGuildChannel;
            if (guildChannel != null)
                prefix = GuildRepo.Get(guildChannel.Guild.Id).Settings.Prefix.ToString();

            var nameLength = _commands.Select(c => c.Name.Length).Max();

            var res = "```";

            foreach (var command in _commands)
            {
                res += $"{prefix}{command.Name.PadRight(nameLength)}  {command.Description}\n";
            }

            res += "```";

            await message.Channel.SendMessageAsync(res);
        }
    }
}
