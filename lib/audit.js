function logMemberJoin(member, settings) {
    if (!settings.audit.member.join) {
        return;
    }

    const channel = member.guild.channels.find(ch => ch.name === settings.audit.channel);
    channel.send(`\`\`\`JOIN: ${member.displayName}\`\`\``);
}

function logMemberLeave(member, settings) {
    if (!settings.audit.member.leave) {
        return;
    }

    const channel = member.guild.channels.find(ch => ch.name === settings.audit.channel);
    channel.send(`\`\`\`LEAVE: ${member.displayName}\`\`\``);
}

function logMemberBan(guild, user, settings) {
    if (!settings.audit.member.ban) {
        return;
    }

    const channel = guild.channels.find(ch => ch.name === settings.audit.channel);
    channel.send(`\`\`\`BAN: ${user.displayName}\`\`\``);
}

function logMemberBanLifted(guild, user, settings) {
    if (!settings.audit.member.banLifted) {
        return;
    }

    const channel = guild.channels.find(ch => ch.name === settings.audit.channel);
    channel.send(`\`\`\`BAN LIFTED: ${user.displayName}\`\`\``);
}

function logMemberNameChange(oldMember, newMember, settings) {
    if (!settings.audit.member.nameChange || oldMember.displayName === newMember.displayName) {
        return;
    }

    const channel = newMember.guild.channels.find(ch => ch.name === settings.audit.channel);
    channel.send(`\`\`\`NAME CHANGE: ${oldMember.displayName} => ${newMember.displayName}\`\`\``);
}

function logSettingsChange(setting, oldValue, newValue, member, settings) {
    if (!settings.audit.settingChange || oldValue === newValue) {
        return;
    }

    const channel = member.guild.channels.find(ch => ch.name === settings.audit.channel);
    channel.send(`\`\`\`SETTING CHANGE: ${setting} ${oldValue} => ${newValue} by ${member.displayName}\`\`\``);
}

module.exports = {
    logMemberJoin: logMemberJoin,
    logMemberLeave: logMemberLeave,
    logMemberNameChange: logMemberNameChange,
    logMemberBan: logMemberBan,
    logMemberBanLifted: logMemberBanLifted,
    logSettingsChange: logSettingsChange
};
