const settings = require('../../settings/settings');
const utils = require('../utils');

module.exports.configureRoleAssignment = function configureRoleAssignment(client) {
    const guildIds = client.guilds.map(g => g.id);
    const guildSettings = settings.get(guildIds);
    for (let guildId in guildSettings) {

        const assignerSettings = guildSettings[guildId].roleAssigner;

        if (!assignerSettings) {
            continue;
        }

        const guild = client.guilds.find(g => g.id === guildId);
        const assignerChannel = guild.channels.find(channel => channel.id === assignerSettings.channel);

        if (!assignerChannel) {
            continue;
        }

        initReactionWatcher(client, assignerChannel, assignerSettings);
    }

    initCommands(client);
}

function initCommands(client) {
    client.on('message', (message) => {
        const guildSettings = settings.get(message.guild.id);
        const assignerSettings = guildSettings.roleAssigner;

        const isRoleAssignmentChannel = guildSettings.roleAssigner && message.channel.id === guildSettings.roleAssigner.channel;
        const hasPermission = !!message.member.roles.find(r => r.id === guildSettings.moderatorRoleId);

        if (!isRoleAssignmentChannel || !hasPermission) {
            return;
        }

        const parts = message.content.split(/\s+/);
        const role = message.mentions.roles.first();

        if (parts[0] === 'add') {
            settings.addAssignableRole(message.guild.id, parts[1], role.id);
            updateAssignerMessage(message.channel, assignerSettings);
        } else if (parts[0] === 'remove') {
            const removed = settings.removeAssignableRole(message.guild.id, role.id);
            updateAssignerMessage(message.channel, assignerSettings)
                .then(newMessage => removeReactions(newMessage, removed.reaction));
        } else {
            return;
        }

        message.delete();
    });
}

function initReactionWatcher(client, assignerChannel, assignerSettings) {
    getAssignerMessage(assignerChannel, assignerSettings)
        .then(assignerMessage => {
            client.on('messageReactionAdd', (reaction, user) => {
                const guildMember = assignerChannel.guild.members.find(m => m.user === user);
                const roleToAdd = assignerSettings.roles.find(r => r.reaction === reaction.emoji.name);
                if (roleToAdd) {
                    guildMember.addRole(roleToAdd.roleId);
                }
            });

            client.on('messageReactionRemove', (reaction, user) => {
                const guildMember = assignerChannel.guild.members.find(m => m.user === user);
                const roleToRemove = assignerSettings.roles.find(r => r.reaction === reaction.emoji.name);
                if (roleToRemove) {
                    guildMember.removeRole(roleToRemove.roleId);
                }
            });
        });
}

function getAssignerMessage(assignerChannel, assignerSettings) {
    if (assignerSettings.messageId) {
        return assignerChannel.fetchMessage(assignerSettings.messageId)
            .then(msg => {
                return msg;
            })
            .catch((err) => {
                const content = createAssignerMessage(assignerChannel.guild, assignerSettings);
                return sendAssignerMessage(assignerChannel, assignerSettings, content);
            });
    } else {
        const content = createAssignerMessage(assignerChannel.guild, assignerSettings);
        return sendAssignerMessage(assignerChannel, assignerSettings, content);
    }
}

function createAssignerMessage(guild, assignerSettings) {
    message = `**Here are the roles you can assign to yourself:**`;
    message += '\n\n';

    assignerSettings.roles.forEach(roleToAdd => {
        const role = guild.roles.find(r => r.id === roleToAdd.roleId);
        message += `${roleToAdd.reaction} -> ${role.toString()}`;
        message += '\n';
    });

    message += '\nRemoving your reaction to for any of these roles will also remove that role.';

    return message;
}

function updateAssignerMessage(assignerChannel, assignerSettings) {
    const newContent = createAssignerMessage(assignerChannel.guild, assignerSettings);
    return getAssignerMessage(assignerChannel, assignerSettings)
        .then(assignerMessage => {
            return assignerMessage.edit(newContent)
        }).then(newAssignerMessage => {
            return utils.addReactions(newAssignerMessage, assignerSettings.roles)
                .then(() => newAssignerMessage);
        });
}

function sendAssignerMessage(assignerChannel, assignerSettings, content) {
    return assignerChannel.send(content)
        .then(assignerMessage => {
            settings.saveRoleAssigmentMessage(assignerChannel.guild.id, assignerMessage);
            return utils.addReactions(assignerMessage, assignerSettings.roles)
                .then(() => assignerMessage);
        });
}

function removeReactions(message, reaction) {
    const reactionToRemove = message.reactions.find(r => r.emoji.name === reaction);
    reactionToRemove.removeAll();
}