const path = require('path');

const settingsManager = require('./settings-manager');
const commands = require('require-all')(path.join(__dirname, 'commands'));
const responses = require('require-all')(path.join(__dirname, 'responses'));

function handleMessage(message) {
    const settings = settingsManager.getGuildSettings(message.guild.id);
    if (message.content.startsWith(settings.commandPrefix)) {
        let parts = message.content.trim().split(/\s+/);
        let commandName = parts[0].substr(1);

        if (commands[commandName]) {
            let command = commands[commandName];
            let args = parts.slice(1);

            command.run(message, args);
        }

        return;
    }

    Object.values(responses).forEach((r) => {
        if (r.mode === 'exact') {
            const modified = r.modifier ? r.modifier(message.content) : message.content;

            if (modified === r.search) {
                r.run(message);
            }
        }
    });
}

module.exports = handleMessage;
