const _ = require('lodash');

module.exports = {
    description: 'Displays a random quote from The Office.',
    run: function(message) {
        const quotes = require('../../quotes/office.json');
        const quote = _.sample(quotes);

        const response = `\`\`\`${quote.quote}`;
        response += `\n\n  - ${quote.author}\`\`\``;

        message.channel.send(response);
    }
};
