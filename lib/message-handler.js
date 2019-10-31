const path = require('path');

const settingsManager = require('./settings-manager');
const sheetsManager = require('./google-sheets-manager');
const commands = require('require-all')(path.join(__dirname, 'commands'));

let responses = {};

function initResponses() {
    sheetsManager.getAutoResponses()
        .then(res => {
            responses = res;
        });
}

function handleMessages(message) {
    const settings = settingsManager.getGuildSettings(message.guild.id);
    if (message.content.startsWith(settings.commandPrefix)) {
        let parts = message.content.trim().split(/\s+/);
        let commandName = parts[0].substr(1);

        if (commandName === 'refresh') {
            return refresh();
        }

        if (commands[commandName]) {
            let command = commands[commandName];
            let args = parts.slice(1);

            return command.run(message, args);
        }
    }

    responses[message.guild.id].forEach((r) => {
        if (r.mode === 'exact') {
            if (message.content === r.search) {
                message.channel.send(r.response);
            }
        } else if (r.mode === 'contains') {
            if (message.content.includes(r.search)) {
                message.channel.send(r.response);
            }
        }
    });
}

function refresh() {
    settingsManager.loadAll();
    initResponses();
    initResponses();
}

module.exports = {
    initResponses: initResponses,
    handleMessages: handleMessages,
    refresh: refresh
};
