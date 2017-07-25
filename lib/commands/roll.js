module.exports = {
    description: 'Rolls an n sided die.',
    run: function(message, args) {
        const _ = require('lodash');
        
        const usage = 'Usage:```!roll <number of sides>```';
        
        if (args.length === 0) {
            message.channel.send(usage);
            return;
        }
        
        const n = parseInt(args.shift());
        
        if (_.isNaN(n)) {
            message.channel.send(usage);
            return;
        }
        
        const result = _.random(1, n);
        
        message.channel.send(`**${result}**`);
    }
};