const Discord = require('discord.js');
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
        const formattedJoinTime = joinTime.format('MMM D, YYYY');

        const registerTime = moment(member.user.createdAt);
        const formattedRegisterTime = registerTime.format('MMM D, YYYY');

        let roles = _
            .chain(member.roles.array())
            .sortBy('calculatedPosition')
            .reverse();

        const embed = new Discord.RichEmbed()
            .setAuthor(member.user.tag, member.user.avatarURL)
            .setDescription(member)
            .setColor(0x00AE86)
            .addField('Joined', formattedJoinTime, true)
            .addField('Registered', formattedRegisterTime, true)
            .setFooter("Don't shoot the messenger.")
            .setTimestamp();

        if (member.lastMessage) {
            embed.addField('Last Message', member.lastMessage, true)
        }

        embed.addField(`Roles [${roles.value().length}]`, roles.join(' '));

        message.channel.send({embed});
    }
};
