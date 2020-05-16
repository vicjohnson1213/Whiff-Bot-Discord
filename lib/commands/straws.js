module.exports = {
    description: 'Draws straws for all users in a voice channel.',
    run: function(message, args) {
        const _ = require('lodash');
        const config = require('../../config/config');
        const usage = 'Usage:```' + config.prefix + 'straws [channel]```';

        const channelName = args.length === 0 ? message.member.voiceChannel.name : args.join(' ');

        const channel = message.guild.channels.find(c => {
            return c.type === 'voice' && c.name.toLowerCase() === channelName.toLowerCase();
        });

        const activeUsers = channel.members.array().map(u => {
            return u.user.username;
        });

        const usernameLengths = activeUsers.map(u => u.length);
        const padSize = _.max(usernameLengths) + 2;

        const shuffled = _.shuffle(activeUsers);

        res = '```';

        shuffled.forEach((u, idx) => {
            const strawSize = activeUsers.length - idx + 3;
            res += `${_.padEnd(u, padSize, ' ')}${_.padEnd('', strawSize, '===')}\n`;
        });

        res += '```';

        if (activeUsers.length > 0) {
            message.channel.send(res);
        } else {
            message.channel.send(usage);
        }
    }
};
