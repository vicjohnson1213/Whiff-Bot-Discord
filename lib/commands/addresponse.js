module.exports = {
    description: 'Adds a new auto-response.',
    run: function(message) {
        const sheetsManager = require('../google-sheets-manager');
        const messageHandler = require('../message-handler');

        const usage = 'Usage:```!addresponse <exact|contains> "search string" "response string"```\n**Note:** Quotes are required.';
        const format = /[!\w]+\s+(\w+)\s+"([^"]+)"\s+"([^"]+)"/;
        const matches = message.content.match(format);

        if (!matches) {
            return message.channel.send(usage);
        }

        const mode = matches[1];
        const search = matches[2];
        const response = matches[3];

        sheetsManager.createAutoResponseForGuild(message.guild.id, mode, search, response)
            .then(() => messageHandler.refresh())
            .catch((err) => console.log(err));

        message.channel.send(':thumbsup:');
    }
};
