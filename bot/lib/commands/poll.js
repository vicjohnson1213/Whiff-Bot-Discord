const _ = require('lodash');
const settings = require('../../../settings/settings');
const utils = require('../../utils');

const numberEmojis = [
    { reactionName: ':zero:', reaction: '0️⃣' },
    { reactionName: ':one:', reaction: '1️⃣' },
    { reactionName: ':two:', reaction: '2️⃣' },
    { reactionName: ':three:', reaction: '3️⃣' },
    { reactionName: ':four:', reaction: '4️⃣' },
    { reactionName: ':five:', reaction: '5️⃣' },
    { reactionName: ':six:', reaction: '6️⃣' },
    { reactionName: ':seven:', reaction: '7️⃣' },
    { reactionName: ':eight:', reaction: '8️⃣' },
    { reactionName: ':nine:', reaction: '9️⃣' }
];

module.exports = {
    description: 'Creates a new poll',
    run: function(message, args, fullArgs) {
        const guildSettings = settings.get(message.guild.id);
        const usage = 'Usage:```' + guildSettings.prefix + 'poll "Question" [option 1|option 2|option 3]```';

        const argsRe = /"([^"]+)"\s+\[([^\]]+)\]/;

        const argMatches = fullArgs.match(argsRe);
        if (!argMatches) {
            message.channel.send(usage);
            return
        }

        const question = argMatches[1];
        const options = argMatches[2].split('|').map(s => s.trim());

        let poll = `**${question}**\n`;
        options.forEach((opt, idx) => {
            poll += numberEmojis[idx].reactionName + ` - ${opt}\n`;
        });

        message.channel.send(poll)
            .then(sent => {
                return utils.addReactions(sent, numberEmojis.slice(0, options.length));
            });
    }
};
