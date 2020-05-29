const path = require('path');

const express = require('express');
const app = express();
const port = 6069;

module.exports.initializeAdminSite = function initializeAdminSite(client) {
    app.get('/guild', getGuilds(client));

    app.use(express.static(path.join(__dirname, 'public')));
    app.listen(port, () => {});
}

function getGuilds(client) {
    return (req, res) => {
        res.json(client.guilds.map(mapGuild));
    };
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

function mapGuildMember(member) {
    return {
        id: member.id,
        name: member.displayName
    }
}