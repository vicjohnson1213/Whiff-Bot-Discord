$(function() {
    createServerList();
});

function createServerList() {
    return new Promise((resolve, reject) => {
        $.get('/client', (res) => {
            $('#client-name').text(res.username);
            let joinedGuilds = $('#joined-guilds');
            res.guilds.forEach(guild => {
                let guildButton = $('<button>');
                guildButton.addClass('guild-button');
                guildButton.text(guild.name);
                joinedGuilds.append(guildButton);

                guildButton.click(() => selectGuild(guild));
            });

            resolve();
        });
    });
}

function selectGuild(guild) {
    $.get(`guild/${guild.id}`, (guildInfo) => {
        console.log('guild info', guildInfo);
        $('#guild-name').text(guildInfo.name);
    });
}