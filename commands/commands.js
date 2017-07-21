const _ = require('lodash');

const commands = require('./');

module.exports = {
    description: 'Displays a list of all available commands.',
    func: function(message) {
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