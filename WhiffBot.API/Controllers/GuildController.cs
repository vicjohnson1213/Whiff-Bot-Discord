using Discord;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhiffBot.Data;
using WhiffBot.Model;

namespace WhiffBot.API.Controllers
{
    [ApiController]
    [Route("guild")]
    public class GuildController : ControllerBase
    {
        private readonly IGuildRepository _guildRepo;
        private readonly IDiscordClient _discord;

        public GuildController(IGuildRepository guildRepo, IDiscordClient discord)
        {
            _guildRepo = guildRepo;
            _discord = discord;
        }

        [HttpGet]
        public List<Guild> GetJoinedGuildIds()
        {
            var guilds = _guildRepo.GetJoinedGuilds();
            return guilds;
        }

        [HttpGet]
        [Route("{guildId}/settings")]
        public GuildSettings GetGuildSettings([FromRoute] ulong guildId)
        {
            var settings = _guildRepo.GetSettings(guildId);
            return settings;
        }

        [HttpGet]
        [Route("{guildId}/responses")]
        public Dictionary<string, string> GetAutoResponses([FromRoute] ulong guildId)
        {
            var responses = _guildRepo.GetAutoResponses(guildId);
            return responses;
        }

        [HttpGet]
        [Route("{guildId}/members")]
        public async Task<List<GuildMember>> GetGuildMembers([FromRoute] ulong guildId)
        {
            var guild = await _discord.GetGuildAsync(guildId);
            var members = await guild.GetUsersAsync();
            return members.Select(m => m.ToGuildMember()).ToList();
        }
    }
}
