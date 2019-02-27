const _ = require('lodash');
const settingsManager = require('../settings-manager');
const audit = require('../audit');

module.exports = {
    description: 'manages the settings for the bot.',
    run: function(message, args) {
        const settings = settingsManager.getGuildSettings(message.guild.id);

        if (!message.member.roles.find(r => r.name === settings.modRole)) {
            return;
        }

        let parts = message.content.trim().split(/\s+/);
        const setting = parts[1];
        let newValue = parts[2];

        if (!setting) {
            return message.channel.send(`\`\`\`${JSON.stringify(settings, ' ', 2)}\`\`\``);
        }

        const oldValue = settingsManager.updateSetting(message.guild, message.member, setting, newValue);

        if (oldValue === null) {
            return message.channel.send(`\`\`\`[${setting}] is not a valid setting.\`\`\``);
        }

        audit.logSettingsChange(setting, oldValue, newValue, message.member, settings);
    }
};
