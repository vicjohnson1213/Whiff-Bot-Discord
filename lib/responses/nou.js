module.exports = {
    search: 'no u',
    mode: 'exact',
    modifier: (message) => message.toLowerCase(),
    run: (message) => {
        message.channel.send('no u');
    }
}
