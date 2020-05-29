module.exports = {
    description: 'Rolls an n sided die.',
    run: function(message, args) {
        const _ = require('lodash');
        const usage = 'Usage:```~roll [number of sides] (defaults to 100)```';
        let max = 100;

        if (args.length > 0) {
            max = parseInt(args.shift());
        }


        if (_.isNaN(max)) {
            message.channel.send(usage);
            return;
        }

        const result = _.random(1, max);

        message.channel.send(`\`\`\`${result}\`\`\``);
    }
};
