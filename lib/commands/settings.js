const _ = require('lodash');
const audit = require('../audit');
const settingsManager = require('../settings-manager');

module.exports = {
    description: 'manages the settings for the bot.',
    run: function(message, args, settings) {
        if (!message.member.roles.find(r => r.name === settings.modRole)) {
            return;
        }

        let parts = message.content.trim().split(/\s+/);
        const setting = parts[1];
        let newValue = parts[2];
        let oldValue = _.get(settings, setting);

        newValue = newValue === 'true' ? true : newValue;
        newValue = newValue === 'false' ? false : newValue;

        if (!setting) {
            return message.channel.send(`\`\`\`${JSON.stringify(settings, ' ', 2)}\`\`\``);
        }

        audit.logSettingsChange(setting, oldValue, newValue, message.member, settings);

        _.set(settings, setting, newValue);

        settingsManager.saveSettings(message.guild, settings);
    }
};
