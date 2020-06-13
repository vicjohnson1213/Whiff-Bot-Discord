const _ = require('lodash');
const settings = require('../../../settings/settings');

module.exports = {
    description: 'Rolls an n sided die.',
    run: function(message, args) {
        console.log(typeof message.guild.id)
        const guildSettings = settings.get(message.guild.id);
        const usage = 'Usage:```' + guildSettings.prefix + 'roll [number of sides] (defaults to 100)```';
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
