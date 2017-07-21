const _ = require('lodash');

module.exports = {
    description: 'Rolls an n sided die.',
    func: function(message, args) {
        if (args.length === 0) {
            message.channel.send('Usage:```!roll <number of sides>```');
            return;
        }
        
        // TODO: Check that this first arg is a number.
        const n = parseInt(args.shift());
        const result = _.random(1, n);
        
        message.channel.send(`**${result}**`);
    }
};