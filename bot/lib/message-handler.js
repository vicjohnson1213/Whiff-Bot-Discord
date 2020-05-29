const path = require('path');
const commands = require('require-all')(path.join(__dirname, 'commands'));

function handleMessages(message) {
    if (message.content.startsWith('~')) {
        let parts = message.content.trim().split(/\s+/);
        let commandName = parts[0].substr(1);
        let fullArgs = parts.slice(1).join(' ');

        if (commands[commandName]) {
            let command = commands[commandName];
            let args = parts.slice(1);

            return command.run(message, args, fullArgs);
        }
    }
}

module.exports = {
    handleMessages: handleMessages,
};
