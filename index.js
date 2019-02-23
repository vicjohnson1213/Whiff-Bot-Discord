const Discord = require('discord.js');

const config = require('./config/config');
const settingsManager = require('./lib/settings-manager');
const audit = require('./lib/audit');
const messageHandler = require('./lib/message-handler');

const client = new Discord.Client();

let guildSettings;

client.on('ready', () => {
    const guildIds = client.guilds.map((guild) => guild.id);
    guildSettings = settingsManager.loadAll(guildIds);
});

client.on('guildMemberAdd', (member) => {
    audit.logMemberJoin(member, guildSettings[member.guild.id]);
});

client.on('guildMemberRemove', (member) => {
    audit.logMemberLeave(member, guildSettings[member.guild.id]);
});

client.on('guildMemberUpdate', (oldMember, newMember) => {
    audit.logMemberNameChange(oldMember, newMember, guildSettings[newMember.guild.id]);
});

client.on('guildBanAdd', (guild, user) => {
    audit.logMemberBan(guild, user, guildSettings[guild.id]);
});

client.on('guildBanRemove', (guild, user) => {
    audit.logMemberBanLifted(guild, user, guildSettings[guild.id]);
});

client.on('message', (message) => {
    messageHandler(message, guildSettings[message.guild.id]);
});

client.login(config.token);
