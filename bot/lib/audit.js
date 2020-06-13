const settings = require('../../settings/settings');

function logGuildUpdate(oldGuild, newGuild) {
    if (oldGuild.name === newGuild.name) {
        return;
    }

    const guildSettings = settings.get(newGuild.id);
    const channel = newGuild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`GUILD NAME CHANGED: [${oldGuild.name}] => [${newGuild.name}]\`\`\``);
    }
}

function logMemberJoin(member) {
    const guildSettings = settings.get(member.guild.id);
    const channel = member.guild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`JOIN: [${member.user.tag}]\`\`\``);
    }
}

function logMemberLeave(member) {
    const guildSettings = settings.get(member.guild.id);
    const channel = member.guild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`LEAVE: [${member.user.tag}]\`\`\``);
    }
}

function logMemberBan(guild, user) {
    const guildSettings = settings.get(guild.id);
    const channel = guild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`BAN: [${user.user.tag}]\`\`\``);
    }
}

function logMemberBanLifted(guild, user) {
    const guildSettings = settings.get(guild.id);
    const channel = guild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`BAN LIFTED: [${user.user.tag}]\`\`\``);
    }
}

function logMemberNameChange(oldMember, newMember) {
    const guildSettings = settings.get(newMember.guild.id);
    if (oldMember.displayName === newMember.displayName) {
        return;
    }

    const channel = newMember.guild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`NAME CHANGE: [${oldMember.user.tag}] => [${newMember.displayName}]\`\`\``);
    }
}

function logChannelCreate(channel) {
    const guildSettings = settings.get(channel.guild.id);
    const auditChannel = channel.guild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (auditChannel) {
        auditChannel.send(`\`\`\`CHANNEL CREATED: [${channel.name}]\`\`\``);
    }
}

function logChannelDelete(channel) {
    const guildSettings = settings.get(channel.guild.id);
    const auditChannel = channel.guild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (auditChannel) {
        auditChannel.send(`\`\`\`CHANNEL DELETED: [${channel.name}]\`\`\``);
    }
}

function logChannelUpdate(oldChannel, newChannel) {
    if (oldChannel.name === newChannel.name) {
        return;
    }

    const guildSettings = settings.get(newChannel.guild.id);
    const auditChannel = newChannel.guild.channels.find(ch => ch.name === guildSettings.auditChannelName);
    if (auditChannel) {
        auditChannel.send(`\`\`\`CHANNEL NAME CHANGED: [${oldChannel.name}] => [${newChannel.name}]\`\`\``);
    }
}

module.exports = {
    logGuildUpdate: logGuildUpdate,
    logMemberJoin: logMemberJoin,
    logMemberLeave: logMemberLeave,
    logMemberNameChange: logMemberNameChange,
    logMemberBan: logMemberBan,
    logMemberBanLifted: logMemberBanLifted,
    logChannelCreate: logChannelCreate,
    logChannelDelete: logChannelDelete,
    logChannelUpdate: logChannelUpdate
};
