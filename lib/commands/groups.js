module.exports = {
    description: 'Randomly assigns people in a voice channel to different groups.',
    run: function(message, args) {
        const _ = require('lodash');
        const usage = 'Usage:```!groups <group size> [channel]```';

        if (args.length === 0) {
            message.channel.send(usage);
            return;
        }

        const groupSize = parseInt(args.shift());

        if (_.isNaN(groupSize) || groupSize <= 0) {
            message.channel.send(usage);
            return;
        }

        const channelName = args.length === 0 ? 'general' : args.join(' ');

        const channel = message.guild.channels.find(c => {
            return c.type === 'voice' && c.name.toLowerCase() === channelName.toLowerCase();
        });

        const activeUsers = channel.members.array().map(u => {
            return u.user.username;
        });

        const shuffled = _.shuffle(activeUsers);
        const groups =  _.chunk(shuffled, groupSize);

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
