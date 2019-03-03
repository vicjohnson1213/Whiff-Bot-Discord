const _ = require('lodash');
const quotes = require('../../quotes/office.json');

module.exports = {
    description: 'Displays a random quote from The Office.',
    run: function(message) {
        let quote = _.sample(quotes);

        let response = `\`\`\`${quote.quote}`;
        response += `\n\n  - ${quote.author}\`\`\``;

        message.channel.send(response);
    }
};
