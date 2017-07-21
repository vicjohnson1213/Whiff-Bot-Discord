const utils = require('../lib/utils');

module.exports = {
    description: 'Randomly assigns people in a voice channel to different groups.',
    func: function(message, args) {
        if (args.length === 0) {
            message.channel.send('Usage:```!groups <group size> [channel]```');
            return;
        }
        
        // TODO: check that this first argument is a number.
        const groupSize = parseInt(args.shift());
        const channelName = args.length === 0 ? 'general' : args.join(' ');
        
        const channel = message.guild.channels.find(c => {
            return c.type === 'voice' && c.name.toLowerCase() === channelName.toLowerCase();
        });
        
        const activeUsers = channel.members.array().map(u => {
            return u.user.username;
        });
        
        const groups = utils.group(activeUsers, groupSize);
        
        let res = '';
        
        groups.forEach((g, i) => {
            res += `\`\`\`${g.join(', ')}\`\`\``;
        });
        
        if (groups.length > 0) {
            message.channel.send(res);
        } else {
            message.channel.send(`There are no users in "${channelName}"`);
        }
    }
};