const path = require('path');

const express = require('express');
const app = express();
const port = 6069;

module.exports.init = function init(client) {
    app.get('/client', getClient(client));
    app.get('/guild/:guildId', getGuildInfo(client));

    app.use(express.static(path.join(__dirname, 'public')));
    app.listen(port, () => {});
}


function getClient(client) {
    return (req, res) => {
        res.json(mapClient(client));
    };
}

function getGuildInfo(client) {
    return (req, res) => {
        console.log(req.params)
        let guild = client.guilds.find(g => g.id === req.params.guildId);
        res.json(mapGuildInfo(guild));
    };
}


function mapClient(client) {
    return {
        avatar: client.user.avatarURL,
        username: client.user.username,
        guilds: client.guilds.map(mapGuild)
    }
}

function mapGuild(guild) {
    return {
        id: guild.id,
        name: guild.name,
        createdAt: guild.createdAt,
        icon: guild.icon,
        owner: mapGuildMember(guild.owner)
    }
}

function mapGuildInfo(guild) {
    let base = mapGuild(guild);
    return {
        ...base,
    };
}

function mapGuildMember(member) {
    return {
        id: member.id,
        name: member.displayName
    }
}