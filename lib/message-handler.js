const path = require('path');
const config = require('../config/config');
const commands = require('require-all')(path.join(__dirname, 'commands'));

function handleMessages(message) {
    if (message.content.startsWith(config.prefix)) {
        let parts = message.content.trim().split(/\s+/);
        let commandName = parts[0].substr(1);

        if (commands[commandName]) {
            let command = commands[commandName];
            let args = parts.slice(1);

            return command.run(message, args);
        }
    }
}

module.exports = {
    handleMessages: handleMessages,
};
