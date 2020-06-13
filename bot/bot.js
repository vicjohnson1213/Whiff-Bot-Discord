const config = require('../config/config');
const audit = require('./lib/audit');
const messageHandler = require('./lib/message-handler');
const settings = require('../settings/settings');
const utils = require('./utils');

module.exports.init = function(client) {
    configureEvents(client)
        .then(() => configureRoleAssignment(client));
}

function configureRoleAssignment(client) {
    const guildIds = client.guilds.map(g => g.id);
    const guildSettings = settings.get(guildIds);
    for (let guildId in guildSettings) {
        const assignerSettings = guildSettings[guildId].roleAssigner;

        if (!assignerSettings) {
            break;
        }

        const guild = client.guilds.find(g => g.id === guildId);
        const assignerChannel = guild.channels.find(channel => channel.name === assignerSettings.channel);

        if (!assignerChannel) {
            break;
        }

        getAssignerMessage(guild, assignerSettings, assignerChannel)
            .then(assignerMessage => {
                const reactionCollector = assignerMessage.createReactionCollector(() => true);

                reactionCollector.on('collect', reaction => {
                    reaction.users.forEach(user => {
                        if (user === client.user) {
                            return;
                        }

                        const guildMember = guild.members.find(m => m.user === user);
                        const roleToAdd = assignerSettings.roles.find(r => r.reaction === reaction.emoji.name);
                        guildMember.addRole(roleToAdd.roleId);
                    });
                });

                return utils.addReactions(assignerMessage, assignerSettings.roles);
            });
    }
}

function getAssignerMessage(guild, assignerSettings, assignerChannel) {
    if (assignerSettings.messageId) {
        return assignerChannel.fetchMessage(assignerSettings.messageId)
            .catch(() => {
                return createAssignerMessage(guild, assignerSettings, assignerChannel);
            });
    } else {
        createAssignerMessage(guild, assignerSettings, assignerChannel);
    }
}

function createAssignerMessage(guild, assignerSettings, assignerChannel) {
    message = `**${assignerSettings.infoMessage}**`;
    message += '\n\n';

    assignerSettings.roles.forEach(roleToAdd => {
        const role = guild.roles.find(r => r.id === roleToAdd.roleId);
        message += `${roleToAdd.reaction} -> ${role.toString()}`;
        message += '\n';
    });

    message += '___';

    return assignerChannel.send(message)
        .then(assignerMessage => {
            settings.saveRoleAssigmentMessage(guild.id, assignerMessage);
            return assignerMessage;
        });
}

function configureEvents(client) {
    const completionPromise = new Promise((resolve, reject) => {
        client.on('ready', () => {
            client.user.setPresence({ game: { name: config.game }});
            resolve();
        });
    });

    client.on('guildUpdate', (oldGuild, newGuild) => {
        audit.logGuildUpdate(oldGuild, newGuild);
    });

    client.on('channelCreate', (channel) => {
        audit.logChannelCreate(channel)
    });

    client.on('channelDelete', (channel) => {
        audit.logChannelDelete(channel)
    });

    client.on('channelUpdate', (oldChannel, newChannel) => {
        audit.logChannelUpdate(oldChannel, newChannel)
    });

    client.on('guildMemberAdd', (member) => {
        audit.logMemberJoin(member);
    });

    client.on('guildMemberRemove', (member) => {
        audit.logMemberLeave(member);
    });

    client.on('guildMemberUpdate', (oldMember, newMember) => {
        audit.logMemberNameChange(oldMember, newMember);
    });

    client.on('guildBanAdd', (guild, user) => {
        audit.logMemberBan(guild, user);
    });

    client.on('guildBanRemove', (guild, user) => {
        audit.logMemberBanLifted(guild, user);
    });

    client.on('message', (message) => {
        if (message.member.user === client.user) {
            return;
        }

        messageHandler.handleMessages(message);
    });

    return completionPromise;
}
