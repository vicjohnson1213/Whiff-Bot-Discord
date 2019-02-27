const Discord = require('discord.js');

const auth = require('./config/auth');
const config = require('./config/config');
const settingsManager = require('./lib/settings-manager');
const audit = require('./lib/audit');
const messageHandler = require('./lib/message-handler');

const client = new Discord.Client();

client.on('ready', () => {
    const guildIds = client.guilds.map((guild) => guild.id);
    settingsManager.loadAll(guildIds);
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
    messageHandler(message);
});

client.login(auth.token);
