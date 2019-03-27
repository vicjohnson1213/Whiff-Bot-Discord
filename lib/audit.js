const settingsManager = require('./settings-manager');

function logGuildUpdate(oldGuild, newGuild) {
    const settings = settingsManager.getGuildSettings(newGuild.id);

    if (!settings.audit.guild.rename) {
        return;
    }

    const channel = newGuild.channels.find(ch => ch.name === settings.auditChannel);
    if (channel) {
        channel.send(`\`\`\`GUILD NAME CHANGED: [${oldGuild.name}] => [${newGuild.name}]\`\`\``);
    }
}

function logMemberJoin(member) {
    const settings = settingsManager.getGuildSettings(member.guild.id);

    if (!settings.audit.member.join) {
        return;
    }

    const channel = member.guild.channels.find(ch => ch.name === settings.auditChannel);
    if (channel) {
        channel.send(`\`\`\`JOIN: [${member.user.tag}]\`\`\``);
    }
}

function logMemberLeave(member) {
    const settings = settingsManager.getGuildSettings(member.guild.id);

    if (!settings.audit.member.leave) {
        return;
    }

    const channel = member.guild.channels.find(ch => ch.name === settings.auditChannel);
    if (channel) {
        channel.send(`\`\`\`LEAVE: [${member.user.tag}]\`\`\``);
    }
}

function logMemberBan(guild, user) {
    const settings = settingsManager.getGuildSettings(guild.id);

    if (!settings.audit.member.ban) {
        return;
    }

    const channel = guild.channels.find(ch => ch.name === settings.auditChannel);
    if (channel) {
        channel.send(`\`\`\`BAN: [${user.user.tag}]\`\`\``);
    }
}

function logMemberBanLifted(guild, user) {
    const settings = settingsManager.getGuildSettings(guild.id);

    if (!settings.audit.member.banLifted) {
        return;
    }

    const channel = guild.channels.find(ch => ch.name === settings.auditChannel);
    if (channel) {
        channel.send(`\`\`\`BAN LIFTED: [${user.user.tag}]\`\`\``);
    }
}

function logMemberNameChange(oldMember, newMember) {
    const settings = settingsManager.getGuildSettings(oldMember.guild.id);

    if (!settings.audit.member.nameChange || oldMember.displayName === newMember.displayName) {
        return;
    }

    const channel = newMember.guild.channels.find(ch => ch.name === settings.auditChannel);
    if (channel) {
        channel.send(`\`\`\`NAME CHANGE: [${oldMember.user.tag}] => [${newMember.displayName}]\`\`\``);
    }
}

function logSettingsChange(setting, oldValue, newValue, member) {
    const settings = settingsManager.getGuildSettings(member.guild.id);

    if (!settings.audit.settingChange || oldValue === newValue) {
        return;
    }

    const channel = member.guild.channels.find(ch => ch.name === settings.auditChannel);
    if (channel) {
        channel.send(`\`\`\`SETTING CHANGE: [${setting}] [${oldValue}] => [${newValue}] by [${member.user.tag}]\`\`\``);
    }
}

function logChannelCreate(channel) {
    const settings = settingsManager.getGuildSettings(channel.guild.id);

    if (!settings.audit.channel.create) {
        return;
    }

    const user = channel.client.user;
    const member = channel.guild.member(user);

    const auditChannel = channel.guild.channels.find(ch => ch.name === settings.auditChannel);
    if (auditChannel) {
        auditChannel.send(`\`\`\`CHANNEL CREATED: [${channel.name}] by [${member.user.tag}]\`\`\``);
    }
}

function logChannelDelete(channel) {
    const settings = settingsManager.getGuildSettings(channel.guild.id);

    if (!settings.audit.channel.delete) {
        return;
    }

    const auditChannel = channel.guild.channels.find(ch => ch.name === settings.auditChannel);
    if (auditChannel) {
        auditChannel.send(`\`\`\`CHANNEL DELETED: [${channel.name}]\`\`\``);
    }
}

function logChannelUpdate(oldChannel, newChannel) {
    const settings = settingsManager.getGuildSettings(newChannel.guild.id);

    if (!settings.audit.channel.rename || oldChannel.name === newChannel.name) {
        return;
    }

    const auditChannel = newChannel.guild.channels.find(ch => ch.name === settings.auditChannel);
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
    logSettingsChange: logSettingsChange,
    logChannelCreate: logChannelCreate,
    logChannelDelete: logChannelDelete,
    logChannelUpdate: logChannelUpdate
};
