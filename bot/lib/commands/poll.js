const config = require('../../../config/config');
const numberEmojis = [
    { str: ':zero:', unicode: '0️⃣' },
    { str: ':one:', unicode: '1️⃣' },
    { str: ':two:', unicode: '2️⃣' },
    { str: ':three:', unicode: '3️⃣' },
    { str: ':four:', unicode: '4️⃣' },
    { str: ':five:', unicode: '5️⃣' },
    { str: ':six:', unicode: '6️⃣' },
    { str: ':seven:', unicode: '7️⃣' },
    { str: ':eight:', unicode: '8️⃣' },
    { str: ':nine:', unicode: '9️⃣' }
];

module.exports = {
    description: 'Creates a new poll',
    run: function(message, args, fullArgs) {
        const _ = require('lodash');
        const usage = 'Usage:```~poll "Question" [option 1|option 2|option 3]```';

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
            poll += numberEmojis[idx].str + ` - ${opt}\n`;
        });

        message.channel.send(poll)
            .then(sent => {
                return addInitialReactions(sent, options.length, 0);
            });

        function addInitialReactions(msg, num, current = 0) {
            if (current == num) {
                return Promise.resolve();
            }

            return msg.react(numberEmojis[current].unicode)
                .then(() => addInitialReactions(msg, num, current + 1));
        }
    }
};
