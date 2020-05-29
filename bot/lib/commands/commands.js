module.exports = {
    description: 'Displays a list of all available commands.',
    run: function(message) {
        const path = require('path');
        const _ = require('lodash');
        const config = require('../../../config/config');

        // Import all commands in this directory, excluding this one.
        const commands = require('require-all')({
            dirname: __dirname,
            filter: (filename) => {
                if (filename === 'commands.js') {
                    return false;
                }

                return `${config.prefix}${path.basename(filename, '.js')}`;
            }
        });

        const commandNames = _.sortBy(Object.keys(commands));
        const commandLengths = commandNames.map(n => n.length);
        const padSize = _.max(commandLengths) + 2;

        let res = 'Available commands:```';

        commandNames.forEach(n => {
            res += `${_.padEnd(n, padSize, ' ')}${commands[n].description}\n`;
        });

        res += '```';

        message.channel.send(res);
    }
};
