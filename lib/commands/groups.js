module.exports = {
    description: 'Randomly assigns people in a voice channel to different groups.',
    run: function(message, args) {
        const _ = require('lodash');
        const usage = 'Usage:```!groups <group count>```';

        if (args.length === 0) {
            message.channel.send(usage);
            return;
        }

        const groupCount = parseInt(args.shift());

        if (_.isNaN(groupCount) || groupCount <= 0) {
            message.channel.send(usage);
            return;
        }

        const channel = message.member.voiceChannel;

        if (!channel) {
            return message.channel.send('You must be in a voice channel to use this command')
        }

        const activeUsers = channel.members.array().map(u => u.user.username);
        const shuffled = _.shuffle(activeUsers);

        const groups = [];
        for (let i = 0; i < shuffled.length; i++) {
            const group = i % groupCount;
            const user = shuffled[i];

            if (!groups[group]) {
                groups[group] = [user];
            } else {
                groups[group].push(user);
            }
        }

        let res = '';

        groups.forEach((g, i) => {
            res += `\`\`\`${g.join(', ')}\`\`\``;
        });

        message.channel.send(res);
    }
};
