const Discord = require('discord.js');

const auth = require('./config/auth');
const config = require('./config/config');
const settingsManager = require('./lib/settings-manager');
const audit = require('./lib/audit');
const messageHandler = require('./lib/message-handler');
const sheetsManager = require('./lib/google-sheets-manager');

const client = new Discord.Client();

sheetsManager.init()
    .then(() => {
        client.login(auth.token);
    });

client.on('ready', () => {
    const guildIds = client.guilds.map((guild) => guild.id);
    settingsManager.loadAll(guildIds);
    messageHandler.initResponses();
    client.user.setPresence({ game: { name: config.game }});
});

client.on('guildUpdate', (oldGuild, newGuild) => {
    audit.logGuildUpdate(oldGuild, newGuild);
});

client.on('channelCreate', (channel) => {
    audit.logChannelCreate(channel)
});

client.on('channelDelete', (channel) => {
    audit.logChannelDelete(channel)
});

client.on('channelUpdate', (oldChannel, newChannel) => {
    audit.logChannelUpdate(oldChannel, newChannel)
});

client.on('guildMemberAdd', (member) => {
    audit.logMemberJoin(member);
});

client.on('guildMemberRemove', (member) => {
    audit.logMemberLeave(member);
});

client.on('guildMemberUpdate', (oldMember, newMember) => {
    audit.logMemberNameChange(oldMember, newMember);
});

client.on('guildBanAdd', (guild, user) => {
    audit.logMemberBan(guild, user);
});

client.on('guildBanRemove', (guild, user) => {
    audit.logMemberBanLifted(guild, user);
});

client.on('message', (message) => {
    if (message.member.user === client.user) {
        return;
    }

    messageHandler.handleMessages(message);
});
