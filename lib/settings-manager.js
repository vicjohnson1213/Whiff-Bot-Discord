const path = require('path');
const fs = require('fs');
const _ = require('lodash');

const config = require('../config/config');

let guildSettings = {};

function loadAllSettings(guildIds) {
    guildIds.forEach((id) => {
        const settingsPath = path.join(config.settings.dir, `${id}.json`);
        guildSettings[id] = require(settingsPath);
    });
}

function saveSettings(guild, settings) {
    const settingsPath = path.join(config.settings.dir, `${guild.id}.json`);

    fs.writeFile(settingsPath, JSON.stringify(settings, ' ', 2), (err) => {})
}

function updateSetting(guild, member, setting, newValue) {
    const settings = getGuildSettings(guild.id);
    let oldValue = _.get(settings, setting);

    if (oldValue === undefined) {
        return null;
    }

    newValue = newValue === 'true' ? true : newValue;
    newValue = newValue === 'false' ? false : newValue;

    _.set(settings, setting, newValue);
    saveSettings(guild, settings);

    return oldValue;
}

function getGuildSettings(guildId) {
    return guildSettings[guildId];
}

module.exports = {
    loadAll: loadAllSettings,
    save: saveSettings,
    getGuildSettings: getGuildSettings,
    updateSetting: updateSetting
};
