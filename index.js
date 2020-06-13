const Discord = require('discord.js');

const auth = require('./config/auth');
const bot = require('./bot/bot');
const admin = require('./admin/server');

const client = new Discord.Client();

client.login(auth.token);

bot.configureEvents(client);
admin.initializeAdminSite(client);