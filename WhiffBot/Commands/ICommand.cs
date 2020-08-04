using Discord.WebSocket;
using System.Threading.Tasks;

namespace WhiffBot.Commands
{
    public interface ICommand
    {
        public string Name { get; }
        public string Description { get; }
        public Task Run(SocketMessage message, string args);
    }
}
