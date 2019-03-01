const moment = require('moment');
const _ = require('lodash');

module.exports = {
    description: 'Displays information about a user.',
    run: function(message, args) {
        let member = message.member;
        const user = message.mentions.users.first();

        if (user) {
            const mentionedMember = message.guild.member(user);
            if (member) {
                member = mentionedMember;
            } else {
                return message.channel.send(`\`\`\`Couldn't find user [${user}]. Be sure to @ them.\`\`\``);
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
