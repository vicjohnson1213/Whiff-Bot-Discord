const config = require('../config/config');

function logGuildUpdate(oldGuild, newGuild) {
    if (oldGuild.name === newGuild.name) {
        return;
    }

    const channel = newGuild.channels.find(ch => ch.name === config.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`GUILD NAME CHANGED: [${oldGuild.name}] => [${newGuild.name}]\`\`\``);
    }
}

function logMemberJoin(member) {
    const channel = member.guild.channels.find(ch => ch.name === config.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`JOIN: [${member.user.tag}]\`\`\``);
    }
}

function logMemberLeave(member) {
    const channel = member.guild.channels.find(ch => ch.name === config.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`LEAVE: [${member.user.tag}]\`\`\``);
    }
}

function logMemberBan(guild, user) {
    const channel = guild.channels.find(ch => ch.name === config.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`BAN: [${user.user.tag}]\`\`\``);
    }
}

function logMemberBanLifted(guild, user) {
    const channel = guild.channels.find(ch => ch.name === config.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`BAN LIFTED: [${user.user.tag}]\`\`\``);
    }
}

function logMemberNameChange(oldMember, newMember) {
    if (oldMember.displayName === newMember.displayName) {
        return;
    }

    const channel = newMember.guild.channels.find(ch => ch.name === config.auditChannelName);
    if (channel) {
        channel.send(`\`\`\`NAME CHANGE: [${oldMember.user.tag}] => [${newMember.displayName}]\`\`\``);
    }
}

function logChannelCreate(channel) {
    const auditChannel = channel.guild.channels.find(ch => ch.name === config.auditChannelName);
    if (auditChannel) {
        auditChannel.send(`\`\`\`CHANNEL CREATED: [${channel.name}]\`\`\``);
    }
}

function logChannelDelete(channel) {
    const auditChannel = channel.guild.channels.find(ch => ch.name === config.auditChannelName);
    if (auditChannel) {
        auditChannel.send(`\`\`\`CHANNEL DELETED: [${channel.name}]\`\`\``);
    }
}

function logChannelUpdate(oldChannel, newChannel) {
    if (oldChannel.name === newChannel.name) {
        return;
    }

    const auditChannel = newChannel.guild.channels.find(ch => ch.name === config.auditChannelName);
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
