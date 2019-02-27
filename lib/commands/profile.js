const moment = require('moment');
const _ = require('lodash');

module.exports = {
    description: 'Displays information about a user.',
    run: function(message, args) {
        let member = message.member;
        let parts = message.content.split(/\s+/);

        if (parts.length > 1) {
            parts.shift();
            const search = parts.join(' ');
            const matches = search.match(/\d+/);
            const snowflake = matches ? matches[0] : '';
            member = message.guild.members.find(m => m.id === snowflake);

            if (!member) {
                return message.channel.send(`\`\`\`Couldn't find user [${search}]. Be sure to @ them.\`\`\``)
            }
        }

        const joinTime = moment(member.joinedAt);
        const formattedTime = joinTime.format('MMM D, YYYY');
        // let roles = _.sortBy(member.roles.array(), 'calculatedPosition');
        let roles = _
            .chain(member.roles.array())
            .sortBy('calculatedPosition')
            .reverse()
            .map(r => r.name)
            .join(', ');

        response = '```';
        response += `\n${member.displayName}`
        response += `\nJoined: ${formattedTime}`;
        response += `\nRoles:  ${roles}`;
        response += '\n```';

        message.channel.send(response);
    }
};
