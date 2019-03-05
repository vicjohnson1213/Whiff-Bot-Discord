const path = require('path');
const fs = require('fs');
const _ = require('lodash');

const config = require('../config/config');
const sheetsManager = require('./google-sheets-manager');

let guildSettings = {};

function loadAllSettings(guildIds) {
    return sheetsManager.getSettings()
        .then(settings => {
            guildSettings = settings;

            if (guildIds) {
                const unknownGuilds = guildIds.filter(id => !Object.keys(guildSettings).includes(id));
                unknownGuilds.forEach(g => {
                    guildSettings[g] = sheetsManager.createSettingsForGuild(g);
                });
            }
        })
        .catch(err => {
            console.error(err);
        });
}

function getGuildSettings(guildId) {
    return guildSettings[guildId];
}

module.exports = {
    loadAll: loadAllSettings,
    getGuildSettings: getGuildSettings
};
