function getEnv() {
    const nodeEnv = process.env.NODE_ENV;

    if (nodeEnv === 'dev') {
        return 'dev';
    }

    return 'prod';
}

auditChannelNames = {
    dev: 'audit-log-test',
    prod: 'audit-log'
};

module.exports = {
    game: 'Whiffing oh so hard',
    env: process.env.NODE_ENV,
    auditChannelName: auditChannelNames[getEnv()]
};
