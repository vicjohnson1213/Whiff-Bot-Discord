const path = require('path');

const settingsManager = require('./settings-manager');
const commands = require('require-all')(path.join(__dirname, 'commands'));

function handleMessage(message, settings) {
    if (message.content.startsWith(settings.commandPrefix)) {
        let parts = message.content.trim().split(/\s+/);
        let commandName = parts[0].substr(1);

        if (commands[commandName]) {
            let command = commands[commandName];
            let args = parts.slice(1);

            command.run(message, args, settings);
        }
    }
}

module.exports = handleMessage;
