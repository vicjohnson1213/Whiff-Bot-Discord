const fs = require('fs');
const path = require('path');

const template = {
    prefix: '!',
    auditChannel: 'audit-log'
};

let settings;

module.exports.get = function(guildIds) {
    if (!settings) {
        settings = {};
        guildIds.forEach(guildId => loadGuildSettings(guildId));
    }

    if (typeof guildIds === 'string') {
        return settings[guildIds];
    }

    return settings;
}

module.exports.saveRoleAssigmentMessage = function(guildId, message) {
    settings[guildId].roleAssigner.messageId = message.id;
    saveGuildSettings(guildId);
    refreshGuildSettings(guildId);
}

module.exports.addAssignableRole = function(guildId, reaction, roleId) {
    settings[guildId].roleAssigner.roles.push({
        reaction: reaction,
        roleId: roleId
    });

    saveGuildSettings(guildId);
    refreshGuildSettings(guildId);
}

module.exports.removeAssignableRole = function(guildId, roleId) {
    const assignerSettings = settings[guildId].roleAssigner;
    const indexToRemove = assignerSettings.roles.findIndex(r => r.roleId === roleId);

    const removed = settings[guildId].roleAssigner.roles.splice(indexToRemove, 1);

    saveGuildSettings(guildId);
    refreshGuildSettings(guildId);

    return removed[0];
}

function loadGuildSettings(guildId) {
    try {
        settings[guildId] = require(`./${guildId}.json`);
    } catch (err) {
        if (err.code === 'MODULE_NOT_FOUND') {
            settings[guildId] = template;
            saveGuildSettings(guildId);
            return;
        }

        throw err;
    }
}

function refreshGuildSettings(guildId) {
    settings[guildId] = require(`./${guildId}.json`);
}

function saveGuildSettings(guildId) {
    const fileName = `${guildId}.json`;
    const content = JSON.stringify(settings[guildId], ' ', 4);
    fs.writeFileSync(path.join(__dirname, fileName), content);
}