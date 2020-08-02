const settings = require('../../settings/settings');

module.exports.configureAutoRoles = function configureAutoRoles(client) {
    const guildIds = client.guilds.map(g => g.id);
    const guildSettings = settings.get(guildIds);

    for (let guildId in guildSettings) {
        const autoRoleSettings = guildSettings[guildId].autoRoles;

        if (!autoRoleSettings) {
            continue;
        }

        const guild = client.guilds.find(g => g.id = guildId);

        initVoiceTimer(guild);
        initUpdateTimer(guild);
    }

    initEvents(client);
}

function initEvents(client) {
    client.on('message', (message) => {
        if (message.member.user === client.user) {
            return;
        }

        const guildSettings = settings.get(message.guild.id);
        const autoRoleSettings = guildSettings.autoRoles;

        if (!autoRoleSettings) {
            return;
        }

        if (!autoRoleSettings.members[message.member.id]) {
            autoRoleSettings.members[message.member.id] = {
                name: message.member.user.username,
                id: member.id,
                messages: 0,
                voiceMinutes: 0
            }
        }

        autoRoleSettings.members[message.member.id].messages += 1;
    });
}

function initVoiceTimer(guild) {
    const guildSettings = settings.get(guild.id);
    const autoRoleSettings = guildSettings.autoRoles;

    setInterval(() => {
        const voiceChannels = guild.channels.filter(c => c.type === 'voice');
        voiceChannels.forEach(channel => {
            if (channel === guild.afkChannel) {
                return;
            }

            channel.members.forEach(member => {
                if (!autoRoleSettings.members[member.id]) {
                    autoRoleSettings.members[member.id] = {
                        name: member.user.username,
                        id: member.id,
                        messages: 0,
                        voiceMinutes: 0
                    }
                }

                autoRoleSettings.members[member.id].voiceMinutes += 1;
            });
        });
    }, autoRoleSettings.voiceInterval);
}

function initUpdateTimer(guild) {
    const guildSettings = settings.get(guild.id);
    const autoRoleSettings = guildSettings.autoRoles;

    setInterval(() => {
        Object.values(autoRoleSettings.members).sort((a, b) => {
            return (a.messages + a.voiceMinutes) - (b.messages + b.voiceMinutes);
        });

        settings.saveGuildSettings(guild.id);

    }, autoRoleSettings.updateInterval);
}