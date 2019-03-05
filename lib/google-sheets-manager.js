const GoogleSpreadsheet = require('google-spreadsheet');
const creds = require('../config/auth.js');
const utils = require('./utils');

const SETTINGS_SHEET = 1;
const RESPONSES_SHEET = 2;

const doc = new GoogleSpreadsheet(creds.sheets.spreadsheet_id);
const data = {
    settings: {},
    responsesa: {}
};

module.exports = {
    init: init,
    getSettings: getSettings,
    createSettingsForGuild: createSettingsForGuild
};

function init() {
    return new Promise((resolve, reject) => {
        doc.useServiceAccountAuth(creds.sheets, function (err) {
            resolve();
        });
    });
}

function getSettings() {
    return new Promise((resolve, reject) => {
        doc.getRows(SETTINGS_SHEET, function (err, rows) {
            const settings = {};

            rows.forEach(r => {
                data.settings[r.guildid] = r;
                settings[r.guildid] = mapRowToSettings(r);
            });

            resolve(settings);
        });
    });
}

function createSettingsForGuild(id) {
    const row = { guildid: id };
    defaultSettings(row);
    doc.addRow(SETTINGS_SHEET, row, (err) => {
        if (err) {
            console.error(err)
        }
    });

    return mapRowToSettings(row);
}

function mapSettingsToRow(settings, row) {
    row.commandprefix = settings.commandPrefix;
    row.modrole = settings.modRole;
    row.auditchannel = settings.auditChannel;

    row.auditsettingchange = settings.audit.settingChange;

    row.auditmemberjoin = settings.audit.member.join;
    row.auditmemberleave = settings.audit.member.leave;
    row.auditmemberban = settings.audit.member.ban;
    row.auditmemberbanlifted = settings.audit.member.banLifted;
    row.auditmembernamechange = settings.audit.member.nameChange;

    row.auditchannelcreate = settings.audit.channel.create;
    row.auditchanneldelete = settings.audit.channel.delete;
    row.auditchannelrename = settings.audit.channel.rename;

    row.auditguildrename = settings.audit.guild.create;
}

function mapRowToSettings(row) {
    return {
        commandPrefix: row.commandprefix,
        modRole: row.modrole,
        auditChannel: row.auditchannel,
        audit: {
            settingChange: utils.bool(row.auditsettingchange),
            member: {
                join: utils.bool(row.auditmemberjoin),
                leave: utils.bool(row.auditmemberleave),
                ban: utils.bool(row.auditmemberban),
                banLifted: utils.bool(row.auditmemberbanlifted),
                nameChange: utils.bool(row.auditmembernamechange)
            },
            channel: {
                create: utils.bool(row.auditchannelcreate),
                delete: utils.bool(row.auditchanneldelete),
                rename: utils.bool(row.auditchannelrename)
            },
            guild: {
                rename: utils.bool(row.auditguildrename)
            }
        }
    }
}

function defaultSettings(row) {
    row.commandprefix = '!';
    row.modrole = 'Mod';
    row.auditchannel = 'audit-log';

    row.auditsettingchange = true;

    row.auditmemberjoin = true;
    row.auditmemberleave = true;
    row.auditmemberban = true;
    row.auditmemberbanlifted = true;
    row.auditmembernamechange = true;

    row.auditchannelcreate = true;
    row.auditchanneldelete = true;
    row.auditchannelrename = true;

    row.auditguildrename = true;
}
