const Discord = require('discord.js');

const commands = require('require-all')(__dirname + '/lib/commands');
const auth = require('./config/auth');

const client = new Discord.Client();

client.on('ready', () => {});

client.on('message', message => {
    if (!message.content.startsWith('!')) {
        return;
    }

    let parts = message.content.split(/\s+/);
    let commandName = parts[0].substr(1);

    if (commands[commandName]) {
        let command = commands[commandName];
        let args = parts.slice(1);

        command.run(message, args);
    } else {
        message.channel.send(`\`${commandName}\` is not a valid command. Use \`!commands\` to see a list of all available commands.`);
    }
});

client.login(auth.token);
