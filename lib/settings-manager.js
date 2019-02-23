const path = require('path');
const fs = require('fs');

const config = require('../config/config');

function loadAllSettings(guildIds) {
    settings = {};

    guildIds.forEach((id) => {
        const settingsPath = path.join(config.settings.dir, `${id}.json`);
        settings[id] = require(settingsPath);
    });

    return settings;
}

function saveSettings(guild, settings) {
    const settingsPath = path.join(config.settings.dir, `${guild.id}.json`);

    fs.writeFile(settingsPath, JSON.stringify(settings, ' ', 2), (err) => {})
}

module.exports = {
    loadAll: loadAllSettings,
    save: saveSettings
};
