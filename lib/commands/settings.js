const settingsManager = require('../settings-manager');
const audit = require('../audit');

module.exports = {
    description: 'Manages the settings for the bot.',
    run: function(message, args) {
        if (args[0] === 'refresh') {
            settingsManager.loadAll()
                .then(sendSettings);
        } else {
            sendSettings();
        }

        function sendSettings() {
            const settings = settingsManager.getGuildSettings(message.guild.id);

            if (!message.member.roles.find(r => r.name === settings.modRole)) {
                return;
            }

            return message.channel.send(`\`\`\`${JSON.stringify(settings, ' ', 2)}\`\`\``);
        }
    }
};
